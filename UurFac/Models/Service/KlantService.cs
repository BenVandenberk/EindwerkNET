using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UurFac.Models.Repo;
using UurFac.Models.UurFac;

namespace UurFac.Models.Service
{
    public class KlantService : IKlantService
    {
        private IUnitOfWork uow;

        public KlantService(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public KlantViewModel KlantViewModel(Gebruiker gebruiker)
        {
            KlantViewModel kvm = new KlantViewModel();
            kvm.Klant = new Klant(); 

            if (gebruiker != null)
            {
                if (gebruiker.Rol == Rol.ADMINISTRATOR)
                {
                    IEnumerable<Departement> alleDepartementen = uow.DepartementRepository.All;
                    kvm.setDepartementen(alleDepartementen);
                }
                if (gebruiker.Rol == Rol.DEPARTEMENT_ADMINISTRATOR)
                {
                    kvm.setDepartementen(gebruiker.Departementen());
                }
            }

            return kvm;
        }

        public IEnumerable<Klant> AlleKlanten(Gebruiker gebruiker)
        {
            List<Klant> alleKlanten = new List<Klant>();

            if (gebruiker != null)
            {
                if (gebruiker.Rol == Rol.ADMINISTRATOR)
                {
                    alleKlanten = uow.KlantRepository.All.ToList();
                }
                if (gebruiker.Rol == Rol.DEPARTEMENT_ADMINISTRATOR)
                {
                    foreach (Departement dep in gebruiker.Departementen())
                    {
                        alleKlanten.AddRange(dep.Klanten());
                    }
                }
            }

            return alleKlanten;
        }

        public void Nieuw(Klant klant)
        {
            uow.KlantRepository.InsertOrUpdate(klant);
            uow.saveChanges();
        }

        // Maakt een nieuwe Klant en koppelt ze direct aan een departement
        public void Nieuw(Klant klant, int departementId)
        {            
            Departement departement = uow.DepartementRepository.Find(departementId);
            DepartementKlant.koppelDepartementAanKlant(departement, klant);
            uow.KlantRepository.InsertOrUpdate(klant);
            uow.saveChanges();
        }

        public IEnumerable<Departement> Departementen(int klantId)
        {
            Klant klant = uow.KlantRepository.Find(klantId);
            return klant.Departementen();
        }

        public DepartementKlant KoppelDepartementAanKlant(int departementId, int klantId)
        {
            Klant klant = uow.KlantRepository.Find(klantId);
            Departement departement = uow.DepartementRepository.Find(departementId);

            if (klant == null || departement == null)
            {
                throw new Exception("Klant of Departement bestaat niet");
            }

            DepartementKlant result = DepartementKlant.koppelDepartementAanKlant(departement, klant);
            uow.saveChanges();

            return result;
        }

        public void OntkoppelDepartementVanKlant(int departementId, int klantId, Gebruiker gebruiker)
        {
            Klant klant = uow.KlantRepository.Find(klantId);
            Departement departement = uow.DepartementRepository.Find(departementId);

            if (klant == null || departement == null)
            {
                throw new Exception("Klant of departement onbekend");
            }
            if (klant.DepartementKlanten.Count < 2)
            {
                throw new Exception("Een klant moet minstens aan één departement gekoppeld zijn");
            }
            if (gebruiker.Rol == Rol.DEPARTEMENT_ADMINISTRATOR)
            {
                if (!gebruiker.Departementen().Contains(departement))
                {
                    throw new Exception("Als departement administrator kan je enkel klanten ontkoppelen van je eigen departementen");
                }
            }

            DepartementKlant ontkoppeld = klant.OntkoppelVanDepartement(departement);
            uow.DepartementKlantRepository.Delete(ontkoppeld.Id);
            uow.saveChanges();
        }

        public IEnumerable<Departement> MogelijkeDepartementenKlantGebruiker(int klantId, Gebruiker gebruiker)
        {
            Klant klant = uow.KlantRepository.Find(klantId);

            if (klant == null || gebruiker == null)
            {
                throw new Exception("Klant of gebruiker onbekend");
            }

            IEnumerable<Departement> result = null;

            IEnumerable<Departement> alleDepartementen = uow.DepartementRepository.All;
            IEnumerable<Departement> klantDepartementen = klant.Departementen();

            if (gebruiker.Rol == Rol.ADMINISTRATOR)
            {
                result = alleDepartementen.Except(klantDepartementen);
            }
            if (gebruiker.Rol == Rol.DEPARTEMENT_ADMINISTRATOR)
            {
                IEnumerable<Departement> gebruikerDepartementen = gebruiker.Departementen();
                result = gebruikerDepartementen.Except(klantDepartementen);
            }

            return result;
        }

        public void Delete(int klantId, Gebruiker gebruiker)
        {
            Klant klant = uow.KlantRepository.Find(klantId);
            if (klant == null)
            {
                throw new Exception("Geen klant gevonden");
            }
            if (!klant.Verwijderbaar())
            {
                throw new Exception("De Klant is niet verwijderbaar omdat er al uurregistraties en/of facturen aan gekoppeld zijn");
            }

            List<DepartementKlant> departementKlanten = klant.DepartementKlanten.ToList();
            List<GebruikerKlant> gebruikerKlanten = klant.GebruikerKlanten();

            if (gebruiker.Rol == Rol.DEPARTEMENT_ADMINISTRATOR)
            {
                IEnumerable<Departement> klantDepartementen = departementKlanten.Select(dk => dk.Departement);
                IEnumerable<Departement> gebruikerDepartementen = gebruiker.Departementen();
                if (klantDepartementen.Except(gebruikerDepartementen).Count() > 0)
                {
                    throw new Exception("Als departement administrator kan je geen klanten verwijderen die koppelingen hebben met departementen waar je niet voor bevoegd bent");
                }
            }

            gebruikerKlanten.Select(gk => gk.Id).ToList().ForEach(
                id => uow.GebruikerKlantRepository.Delete(id)
                );

            departementKlanten.Select(dk => dk.Id).ToList().ForEach(
                id => uow.DepartementKlantRepository.Delete(id)
                );

            uow.KlantRepository.Delete(klantId);
            uow.saveChanges();
        }

        public void Dispose()
        {
            uow.Dispose();
        }
    }

    public interface IKlantService : IDisposable
    {
        KlantViewModel KlantViewModel(Gebruiker gebruiker);
        IEnumerable<Klant> AlleKlanten(Gebruiker gebruiker);
        IEnumerable<Departement> Departementen(int klantId);
        void Nieuw(Klant klant);
        void Nieuw(Klant klant, int departementId);        
        DepartementKlant KoppelDepartementAanKlant(int departementId, int klantId);
        void OntkoppelDepartementVanKlant(int departementId, int klantId, Gebruiker gebruiker);
        IEnumerable<Departement> MogelijkeDepartementenKlantGebruiker(int klantId, Gebruiker gebruiker);
        void Delete(int klantId, Gebruiker gebruiker);
    }
}