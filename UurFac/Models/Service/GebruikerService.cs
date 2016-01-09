using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using UurFac.Models.Repo;
using UurFac.Models.UurFac;
using Microsoft.AspNet.Identity;

namespace UurFac.Models.Service
{
    public class GebruikerService : IGebruikerService
    {
        private IUnitOfWork uow;        

        public GebruikerService(IUnitOfWork uow)
        {
            this.uow = uow;            
        }

        public void Ontkoppel(int gebruikerId, int departementId)
        {
            Gebruiker gebruiker = uow.GebruikerRepository.Find(gebruikerId);
            Departement departement = uow.DepartementRepository.Find(departementId);

            if (gebruiker == null || departement == null)
            {
                throw new Exception("Gebruiker en/of departement niet gevonden");
            }

            GebruikerDepartement ontkoppeld = gebruiker.ontkoppelVanDepartement(departement);
            uow.GebruikerDepartementRepository.Delete(ontkoppeld.Id);           

            uow.saveChanges();
        }

        public void Delete(int gebruikerId)
        {
            Gebruiker gebruiker = uow.GebruikerRepository.Find(gebruikerId);

            if (gebruiker == null)
            {
                throw new Exception("Ongeldige gebruikerId");
            }
            if(!gebruiker.Verwijderbaar())
            {
                throw new Exception("Deze gebruiker is niet verwijderbaar omdat er reeds uurregistraties aan gekoppeld zijn");
            }
            if(gebruiker.Equals(LoggedIn()))
            {
                throw new Exception("Verwijderen van eigen account niet mogelijk");
            }


            List<GebruikerDepartement> gebruikerDepartementen = gebruiker.GebruikerDepartementen.ToList();
            List<GebruikerKlant> gebruikerKlanten = gebruiker.GebruikerKlanten();

            gebruikerKlanten.Select(gk => gk.Id).ToList().ForEach(
                id => uow.GebruikerKlantRepository.Delete(id)
                );

            gebruikerDepartementen.Select(gd => gd.Id).ToList().ForEach(
                id => uow.GebruikerDepartementRepository.Delete(id)
                );
              
            uow.GebruikerRepository.Delete(gebruikerId);
            ApplicationUser appUser = uow.Context.Users.Single(u => u.GebruikerId == gebruikerId);
            uow.Context.Users.Remove(appUser);
            uow.saveChanges();
        }

        public void Nieuw(Gebruiker gebruiker)
        {   
            ApplicationUser appUser = new ApplicationUser()
            {                
                UserName = gebruiker.Login       
            };
            uow.IdentityManager.CreateUser(appUser, gebruiker.Login); 

            gebruiker.User = appUser;
            uow.GebruikerRepository.InsertOrUpdate(gebruiker);
            uow.saveChanges();

            uow.IdentityManager.AddUserToRole(appUser.Id, gebruiker.rolString());
            appUser.GebruikerId = gebruiker.Id;
            uow.saveChanges();
        }

        public void Update(Gebruiker gebruiker)
        {
            // Login mag niet geüpdatet worden waardoor de Login property van het argument gebruiker 'null' is
            // => Eerst oude Login waarde terug zetten
            Gebruiker oud = uow.Context.Gebruikers.AsNoTracking().Single(g => g.Id == gebruiker.Id);
            gebruiker.Login = oud.Login;               

            uow.GebruikerRepository.InsertOrUpdate(gebruiker);

            ApplicationUser appUser = null;
            try {
                appUser = uow.Context.Users.Single(u => u.GebruikerId == gebruiker.Id);
            } catch (Exception ex)
            {
                throw new Exception(String.Format("Fout met relatie Gebruiker - User: {0}", ex.Message));
            }

            if (appUser != null)
            {
                uow.IdentityManager.ClearUserRoles(appUser.Id);
                uow.IdentityManager.AddUserToRole(appUser.Id, gebruiker.rolString());
            }

            uow.saveChanges();
        }

        public void UpdateGegevens(string email, string tel, string gsm)
        {
            Gebruiker loggedIn = LoggedIn();

            loggedIn.Email = email;
            loggedIn.Gsm = gsm;
            loggedIn.Tel = tel;

            uow.GebruikerRepository.InsertOrUpdate(loggedIn);
            uow.saveChanges();
        }

        public Gebruiker LoggedIn()
        {
            IIdentity identity = System.Web.HttpContext.Current.User.Identity;
            if (!identity.IsAuthenticated)
            {
                return null;
            }

            string id = identity.GetUserId();
            ApplicationUser appUser = uow.Context.Users.FirstOrDefault(u => u.Id == id);
            return appUser.Gebruiker;
        }

        public IEnumerable<Departement> ListMogelijkeDepartementen(int gebruikerId)
        {
            Gebruiker gebruiker = uow.GebruikerRepository.Find(gebruikerId);

            if (gebruiker != null)
            {
                IEnumerable<Departement> departementen = gebruiker.Departementen();
                IEnumerable<Departement> alleDepartementen = uow.DepartementRepository.All;
                return alleDepartementen.Except(departementen);
            } else
            {
                throw new Exception("Geen gebruiker gevonden");
            }
        }

        public Departement Koppel(int gebruikerId, int departementId)
        {
            Gebruiker gebruiker = uow.GebruikerRepository.Find(gebruikerId);
            Departement departement = uow.DepartementRepository.Find(departementId);

            if (gebruiker == null)
            {
                throw new Exception("Koppeling mislukt: geen gebruiker gevonden");
            }
            if (departement == null)
            {
                throw new Exception("Koppeling mislukt: geen departement gevonden");
            }

            GebruikerDepartement.koppelGebruikerAanDepartement(gebruiker, departement);
            uow.saveChanges();

            return departement;
        }

        public void Dispose()
        {
            uow.Dispose();
        }
    }

    public interface IGebruikerService : IDisposable
    {
        void Ontkoppel(int gebruikerId, int departementId);
        void Delete(int gebruikerId);
        void Nieuw(Gebruiker gebruiker);
        void Update(Gebruiker gebruiker);
        Gebruiker LoggedIn();
        IEnumerable<Departement> ListMogelijkeDepartementen(int gebruikerId);
        Departement Koppel(int gebruikerId, int departementId);
        void UpdateGegevens(string email, string tel, string gsm);
    }
}