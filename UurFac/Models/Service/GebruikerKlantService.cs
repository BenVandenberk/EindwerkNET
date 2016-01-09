using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UurFac.Models.Repo;
using UurFac.Models.UurFac;

namespace UurFac.Models.Service
{
    public class GebruikerKlantService : IGebruikerKlantService
    {
        private IUnitOfWork uow;

        public GebruikerKlantService(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public GebruikerKlantViewModel GebruikerKlantViewModel(Gebruiker gebruiker)
        {
            GebruikerKlantViewModel gkvm = new GebruikerKlantViewModel();

            if(gebruiker != null)
            {
                if (gebruiker.Rol == Rol.ADMINISTRATOR)
                {
                    gkvm.setDepartementen(uow.DepartementRepository.All);
                }
                if (gebruiker.Rol == Rol.DEPARTEMENT_ADMINISTRATOR)
                {
                    gkvm.setDepartementen(gebruiker.Departementen());
                }
            }

            return gkvm;
        }

        public IEnumerable<GebruikerDepartement> GebruikersVan(int departementId)
        {
            Departement departement = uow.DepartementRepository.Find(departementId);

            if (departement != null)
            {
                return departement.GebruikerDepartementen;
            } else
            {
                throw new Exception("Onbestaand departement");
            }            
        }

        public IEnumerable<GebruikerKlant> KlantenVan(int gebruikerDepartementId)
        {
            GebruikerDepartement gebruikerDepartement = uow.GebruikerDepartementRepository.Find(gebruikerDepartementId);

            if (gebruikerDepartement != null)
            {
                return gebruikerDepartement.GebruikerKlanten;
            } else
            {
                throw new Exception("Onbestaand GebruikerDepartement");
            }
        }

        public IEnumerable<DepartementKlant> MogelijkeKlantenVoor(int gebruikerDepartementId)
        {
            GebruikerDepartement gebruikerDepartement = uow.GebruikerDepartementRepository.Find(gebruikerDepartementId);

            if (gebruikerDepartement != null)
            {
                IEnumerable<DepartementKlant> alleDepartementKlanten = gebruikerDepartement.Departement.DepartementKlanten;
                IEnumerable<DepartementKlant> reedsGekoppeld = gebruikerDepartement.DepartementKlanten();
                return alleDepartementKlanten.Except(reedsGekoppeld);
            }
            else
            {
                throw new Exception("Onbestaand GebruikerDepartement");
            }
        }

        public GebruikerKlant KoppelGebruikerAanKlant(int gebruikerDepartementId, int departementKlantId)
        {
            DepartementKlant departementKlant = uow.DepartementKlantRepository.Find(departementKlantId);
            GebruikerDepartement gebruikerDepartement = uow.GebruikerDepartementRepository.Find(gebruikerDepartementId);

            if (departementKlant == null || gebruikerDepartement == null)
            {
                throw new Exception("Ongeldige gebruiker of klant");
            }

            GebruikerKlant result = GebruikerKlant.koppelGebruikerAanKlant(gebruikerDepartement, departementKlant);
            uow.saveChanges();

            return result;
        }   

        public void OntkoppelGebruikerVanKlant(int gebruikerKlantId)
        {
            GebruikerKlant koppeling = uow.GebruikerKlantRepository.Find(gebruikerKlantId);
            
            if (koppeling != null)
            {
                koppeling.ontkoppel();
                uow.GebruikerKlantRepository.Delete(koppeling.Id);
                uow.saveChanges();
            } else
            {
                throw new Exception("Ongeldige gebruikerklantID");
            }
        }

        public void Dispose()
        {
            uow.Dispose();
        }
    }

    public interface IGebruikerKlantService : IDisposable
    {
        GebruikerKlantViewModel GebruikerKlantViewModel(Gebruiker gebruiker);
        IEnumerable<GebruikerDepartement> GebruikersVan(int departementId);
        IEnumerable<GebruikerKlant> KlantenVan(int gebruikerDepartementId);
        IEnumerable<DepartementKlant> MogelijkeKlantenVoor(int gebruikerDepartementId);
        GebruikerKlant KoppelGebruikerAanKlant(int gebruikerDepartementId, int departementKlantId);        
        void OntkoppelGebruikerVanKlant(int gebruikerKlantId);
    }
}