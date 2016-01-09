using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UurFac.Models.Repo;
using UurFac.Models.UurFac;

namespace UurFac.Models.Service
{
    public class FactuurService : IFactuurService
    {
        private IUnitOfWork uow;

        public FactuurService(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public FactuurViewModel FactuurViewModel(int factuurId, bool teBevestigen)
        {
            Factuur factuur = uow.FactuurRepository.Find(factuurId);
            FactuurViewModel fvm = new FactuurViewModel();

            if (factuur != null)
            {
                fvm.Factuur = factuur;
                fvm.Klant = factuur.DepartementKlant.Klant;
                fvm.Departement = factuur.DepartementKlant.Departement;
                fvm.TeBevestigen = teBevestigen;
                fvm.FactuurDetails = factuur.FactuurDetails.ToList();
            }            

            return fvm;
        }

        public IEnumerable<Departement> AlleMogelijkeDepartementen(Gebruiker gebruiker)
        {
            List<Departement> departementen = new List<Departement>();
            IEnumerable<DepartementKlant> alleDepartementKlanten = uow.DepartementKlantRepository.All;

            if (gebruiker != null)
            {
                if (gebruiker.Rol == Rol.ADMINISTRATOR)
                {
                    departementen.AddRange(alleDepartementKlanten.Select(dk => dk.Departement).Distinct());
                }
                if (gebruiker.Rol == Rol.DEPARTEMENT_ADMINISTRATOR)
                {
                    IEnumerable<Departement> depsVanGebruiker = gebruiker.Departementen();
                    departementen.AddRange(
                        alleDepartementKlanten.Select(dk => dk.Departement)
                                              .Where(d => depsVanGebruiker.Contains(d))
                                              .Distinct());
                }
            }

            return departementen;
        }

        public IEnumerable<DepartementKlant> AlleDepartementKlantenVoor(int departementId)
        {
            Departement departement = uow.DepartementRepository.Find(departementId);

            if(departement == null)
            {
                return new List<DepartementKlant>();
            }

            return departement.DepartementKlanten;
        }

        public IEnumerable<UurRegistratie> ZoekUurRegistraties(int depKlantId, DateTime? van, DateTime? tot)
        {
            DepartementKlant departementKlant = uow.DepartementKlantRepository.Find(depKlantId);
            List<UurRegistratie> uurRegs = new List<UurRegistratie>();

            if (departementKlant != null)
            {
                foreach (GebruikerKlant gk in departementKlant.GebruikerKlanten)
                {
                    uurRegs.AddRange(gk.UurRegistraties);
                }
                uurRegs = uurRegs.Where(uur => !uur.isGefactureerd() && uur.isInPeriode(van, tot)).ToList();
            }

            return uurRegs;
        }

        public Factuur Factureer(int[] ids, int departementKlantId)
        {
            DepartementKlant departementKlant = uow.DepartementKlantRepository.Find(departementKlantId);
            Factuur factuur = null;

            if (departementKlant != null && ids.Length > 0)
            {
                factuur = new Factuur()
                {
                    FactuurJaar = DateTime.Today.Year,
                    FactuurDatum = DateTime.Today,
                    FactuurNummer = String.Format("FAC{0}{1}{2}.{3}",
                        DateTime.Today.Year,
                        DateTime.Today.Month,
                        DateTime.Today.Day,
                        departementKlant.Id
                    )
                };

                uow.FactuurRepository.InsertOrUpdate(factuur);
                departementKlant.Facturen.Add(factuur);                
            } else
            {
                throw new Exception("Ongeldige uurregistratieIds of departementKlantId");
            }            

            UurRegistratie uurRegistratie;
            foreach (int id in ids)
            {
                uurRegistratie = uow.UurRegistratieRepository.Find(id);

                if (uurRegistratie != null)
                { 
                    FactuurDetail facDetail = new FactuurDetail()
                    {
                        Factuur = factuur,
                        FactuurId = factuur.Id,
                        Omschrijving = uurRegistratie.Omschrijving,
                        UurRegistratie = uurRegistratie,
                        UurRegistratieId = uurRegistratie.Id
                    };                    

                    facDetail.BerekenLijnWaarde();
                    factuur.FactuurDetails.Add(facDetail);
                }
            }

            factuur.BerekenTotaal();
            uow.saveChanges();

            // Factuurnummer uniek maken

            factuur.FactuurNummer += "." + factuur.Id;
            uow.saveChanges();

            return factuur;
        }

        public void AnnuleerFactuur(int factuurId)
        {
            uow.FactuurRepository.Delete(factuurId);
            uow.saveChanges();
        }

        public IEnumerable<Factuur> AlleFacturenVoor(int departementId)
        {
            Departement departement = uow.DepartementRepository.Find(departementId);

            if (departement != null)
            {
                List<Factuur> facturen = new List<Factuur>();
                foreach(DepartementKlant dk in departement.DepartementKlanten)
                {
                    facturen.AddRange(dk.Facturen);
                }
                return facturen;
            } else
            {
                throw new Exception("Ongeldige departementId");
            }
        }

        public void Dispose()
        {
            uow.Dispose();
        }
    }

    public interface IFactuurService : IDisposable
    {
        FactuurViewModel FactuurViewModel(int factuurId, bool teBevestigen);
        IEnumerable<Departement> AlleMogelijkeDepartementen(Gebruiker gebruiker);
        IEnumerable<DepartementKlant> AlleDepartementKlantenVoor(int departementId);
        IEnumerable<UurRegistratie> ZoekUurRegistraties(int depKlantId, DateTime? van, DateTime? tot);
        Factuur Factureer(int[] ids, int departementKlantId);
        void AnnuleerFactuur(int factuurId);
        IEnumerable<Factuur> AlleFacturenVoor(int departementId);
    }
}