using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UurFac.Models.Repo;
using UurFac.Models.UurFac;

namespace UurFac.Models.Service
{
    public class UurRegistratieService : IUurRegistratieService
    {
        private IUnitOfWork uow;

        public UurRegistratieService(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public IUnitOfWork Uow
        {
            get
            {
                return uow;
            }
        }

        public UurRegistratieViewModel UurRegistratieViewModel(Gebruiker gebruiker, bool gefactureerd)
        {
            UurRegistratieViewModel gkvm = new UurRegistratieViewModel();
            gkvm.Gefactureerd = gefactureerd;

            if (gebruiker != null)
            {
                if (gebruiker.Rol == Rol.ADMINISTRATOR)
                {
                    gkvm.setDepartementen(uow.DepartementRepository.All);
                }
                if (gebruiker.Rol == Rol.DEPARTEMENT_ADMINISTRATOR || gebruiker.Rol == Rol.USER)
                {
                    gkvm.setDepartementen(gebruiker.Departementen());
                }
            }

            return gkvm;
        }

        public IEnumerable<GebruikerDepartement> alleGebruikerDepartementenVoor(Gebruiker gebruiker, int departementId)
        {
            IEnumerable<GebruikerDepartement> gebruikerDepartementen = new List<GebruikerDepartement>();
            Departement departement = uow.DepartementRepository.Find(departementId);

            if (gebruiker != null && departement != null)
            {
                if (gebruiker.Rol == Rol.ADMINISTRATOR || gebruiker.Rol == Rol.DEPARTEMENT_ADMINISTRATOR)
                {
                    gebruikerDepartementen = departement.GebruikerDepartementen;
                }
                if (gebruiker.Rol == Rol.USER)
                {
                    gebruikerDepartementen = gebruiker.GebruikerDepartementen.Where(gd => gd.Departement.Equals(departement));
                }
            }

            return gebruikerDepartementen;            
        }

        public IEnumerable<GebruikerKlant> alleGebruikerKlantenVoor(int gebruikerDepartemenId)
        {
            GebruikerDepartement gebruikerDepartement = uow.GebruikerDepartementRepository.Find(gebruikerDepartemenId);            

            if (gebruikerDepartement != null)
            {
                return gebruikerDepartement.GebruikerKlanten;
            } else
            {
                throw new Exception("Geen gebruikerdepartement gevonden");
            }
        }

        public IEnumerable<UurRegistratie> alleUurRegistratiesVoor(int gebruikerKlantId, bool gefactureerd)
        {
            GebruikerKlant gebruikerKlant = uow.GebruikerKlantRepository.Find(gebruikerKlantId);

            if (gebruikerKlant != null)
            {
                return gebruikerKlant.UurRegistraties.Where(uur => uur.isGefactureerd() == gefactureerd);
            } else
            {
                throw new Exception("Geen gebruikerKlant gevonden");
            }
        }

        public void Nieuw(int gebruikerKlantId, UurRegistratie uurRegistratie)
        {
            GebruikerKlant gebruikerKlant = uow.GebruikerKlantRepository.Find(gebruikerKlantId);

            if (gebruikerKlant != null && uurRegistratie != null)
            {
                gebruikerKlant.UurRegistraties.Add(uurRegistratie);
                uow.saveChanges();
            }
            else
            {
                throw new Exception("Ongeldige operatie: geen gebruikerklant gespecifieerd en/of ongeldige uurregistratie");
            }
        }

        public void Update(UurRegistratie uurRegistratie) {
            if (uurRegistratie != null)
            {
                UurRegistratie dbVersie = uow.UurRegistratieRepository.Find(uurRegistratie.Id);
                if (dbVersie.isGefactureerd())
                {
                    throw new InvalidOperationException("Kan deze uurregistratie niet aanpassen omdat ze reeds gefactureerd is");
                }

                dbVersie.Titel = uurRegistratie.Titel;
                dbVersie.Omschrijving = uurRegistratie.Omschrijving;
                uurRegistratie = null;

                uow.UurRegistratieRepository.InsertOrUpdate(dbVersie);
                uow.saveChanges();
            } else
            {
                throw new NullReferenceException("Geen uurregistratie geselecteerd");
            }
        }

        public void Delete(UurRegistratie uurRegistratie)
        {
            if (uurRegistratie == null)
            {
                throw new NullReferenceException("Geen uurregistratie geselecteerd");
            }
            UurRegistratie dbVersie = uow.UurRegistratieRepository.Find(uurRegistratie.Id);
            uurRegistratie = null;
            
            if (dbVersie.isGefactureerd())
            {
                throw new InvalidOperationException("Deze uurregistratie kan niet verwijderd worden omdat er reeds factuurgegevens aan gekoppeld zijn");
            }

            uow.UurRegistratieRepository.Delete(dbVersie.Id);
            uow.saveChanges();
        }

        public UurRegistratieDetailViewModel UurRegistratieDetailViewModel(int uurRegistratieId, bool gefactureerd)
        {
            UurRegistratie uurRegistratie = uow.UurRegistratieRepository.Find(uurRegistratieId);
            UurRegistratieDetailViewModel urdvm = null;

            if (uurRegistratie != null)
            {
                string gebruiker = String.Format(
                    "{0} {1}", 
                    uurRegistratie.GebruikerKlant.GebruikerDepartement.Gebruiker.Voornaam,
                    uurRegistratie.GebruikerKlant.GebruikerDepartement.Gebruiker.Achternaam);
                string klant = uurRegistratie.GebruikerKlant.Klant().Bedrijfsnaam;
                string departement = uurRegistratie.GebruikerKlant.GebruikerDepartement.Departement.Omschrijving;

                urdvm = new UurRegistratieDetailViewModel(
                    gebruiker,
                    klant,
                    departement,
                    uurRegistratie,
                    gefactureerd);
            }

            return urdvm;
        }

        public IEnumerable<UurRegistratieDetail> alleDetailsVoor(int uurRegistratieId)
        {
            UurRegistratie uurRegistratie = uow.UurRegistratieRepository.Find(uurRegistratieId);

            if (uurRegistratie != null)
            {
                return uurRegistratie.UurRegistratieDetails;
            } else
            {
                throw new Exception("Geen uurregistratie gevonden");
            }
        }

        public void NieuwDetail(int uurRegistratieId, UurRegistratieDetail uurRegistratieDetail, int tariefId)
        {
            UurRegistratie uurRegistratie = uow.UurRegistratieRepository.Find(uurRegistratieId);
            if (uurRegistratie.isGefactureerd())
            {
                throw new InvalidOperationException("Kan geen nieuw detail toevoegen aan deze uurregistratie omdat ze reeds gefactureerd is");
            }

            Tarief tarief = uow.TariefRepository.Find(tariefId);

            if (uurRegistratie != null && uurRegistratieDetail != null && tarief != null)
            {
                uurRegistratieDetail.Tarief = tarief;
                uurRegistratie.UurRegistratieDetails.Add(uurRegistratieDetail);
                uow.saveChanges();
            }
            else
            {
                throw new Exception("Ongeldige operatie: geen uurregistratie gespecifieerd en/of ongeldige uurregistratiedetail");
            }
        }

        public IEnumerable<Tarief> alleTarieven()
        {
            return uow.TariefRepository.All;
        }

        public void DeleteDetail(int uurRegistratieDetailId)
        {
            UurRegistratieDetail uurRegistratieDetail = uow.UurRegistratieDetailRepository.Find(uurRegistratieDetailId);

            if (uurRegistratieDetail == null)
            {
                throw new NullReferenceException("Geen uurregistratiedetail geselecteerd");
            } 
            if (uurRegistratieDetail.isGefactureerd())
            {
                throw new InvalidOperationException("Dit uurregistratiedetail kan niet verwijderd worden omdat er reeds factuurgegevens aan gekoppeld zijn");
            }

            uow.UurRegistratieDetailRepository.Delete(uurRegistratieDetail.Id);
            uow.saveChanges();
        }

        public void UpdateDetail(UurRegistratieDetail uurRegistratieDetail)
        {
            if (uurRegistratieDetail == null)
            {
                throw new NullReferenceException("Geen uurregistratiedetail geselecteerd");
            }

            UurRegistratieDetail dbVersie = uow.UurRegistratieDetailRepository.Find(uurRegistratieDetail.Id);
            if (dbVersie.isGefactureerd())
            {
                throw new InvalidOperationException("Dit uurregistratiedetail kan niet aangepast worden omdat er reeds factuurgegevens aan gekoppeld zijn");
            }
                        
            dbVersie.EindTijd = uurRegistratieDetail.EindTijd;
            dbVersie.StartTijd = uurRegistratieDetail.StartTijd;
            dbVersie.TeFactureren = uurRegistratieDetail.TeFactureren;

            Tarief nieuwTarief = uow.TariefRepository.Find(uurRegistratieDetail.TariefId);
            dbVersie.Tarief = nieuwTarief;
            uurRegistratieDetail = null;

            uow.UurRegistratieDetailRepository.InsertOrUpdate(dbVersie);
            uow.saveChanges();
        }

        public void Dispose()
        {
            uow.Dispose();
        }
    }

    public interface IUurRegistratieService : IDisposable
    {
        UurRegistratieViewModel UurRegistratieViewModel(Gebruiker gebruiker, bool gefactureerd);
        IEnumerable<GebruikerDepartement> alleGebruikerDepartementenVoor(Gebruiker gebruiker, int departementId);
        IEnumerable<GebruikerKlant> alleGebruikerKlantenVoor(int gebruikerDepartemenId);
        IEnumerable<UurRegistratie> alleUurRegistratiesVoor(int gebruikerKlantId, bool gefactureerd);
        void Nieuw(int gebruikerKlantId, UurRegistratie uurRegistratie);
        void Update(UurRegistratie uurRegistratie);
        void Delete(UurRegistratie uurRegistratie);
        IUnitOfWork Uow {get; }
        UurRegistratieDetailViewModel UurRegistratieDetailViewModel(int uurRegistratieId, bool gefactureerd);
        IEnumerable<UurRegistratieDetail> alleDetailsVoor(int uurRegistratieId);
        void NieuwDetail(int uurRegistratieId, UurRegistratieDetail uurRegistratieDetail, int tariefId);
        IEnumerable<Tarief> alleTarieven();
        void DeleteDetail(int uurRegistratieDetailId);
        void UpdateDetail(UurRegistratieDetail uurRegistratieDetail);
    }
}