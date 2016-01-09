
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using UurFac.Models.UurFac;

namespace UurFac.Models
{

    public class KlantViewModel
    {
        private IEnumerable<Departement> departementen = new List<Departement>();  

        public Klant Klant { get; set; }
        [Display(Name = "Initiële departement")]
        public Departement GekozenDepartement { get; set; }
        public IEnumerable<SelectListItem> Departementen
        {
            get
            {
                return new SelectList(departementen, "Id", "Omschrijving");
            }            
        }

        public void setDepartementen(IEnumerable<Departement> departementen)
        {
            this.departementen = departementen;
        }

    }

    public class GebruikerKlantViewModel
    {
        private IEnumerable<Departement> departementen = new List<Departement>();

        [Display(Name = "Departement")]
        public Departement GekozenDepartement { get; set; }
        public IEnumerable<SelectListItem> Departementen
        {
            get
            {
                return new SelectList(departementen, "Id", "Omschrijving");
            }
        }

        public void setDepartementen(IEnumerable<Departement> departementen)
        {
            this.departementen = departementen;
        }
    }

    public class UurRegistratieViewModel
    {
        private IEnumerable<Departement> departementen = new List<Departement>();

        [Display(Name = "Departement")]
        public Departement GekozenDepartement { get; set; }
        public IEnumerable<SelectListItem> Departementen
        {
            get
            {
                return new SelectList(departementen, "Id", "Omschrijving");
            }
        }

        public bool Gefactureerd { get; set; }

        public void setDepartementen(IEnumerable<Departement> departementen)
        {
            this.departementen = departementen;
        }
    }

    public class UurRegistratieDetailViewModel
    {
        public string Gebruiker { get; set; }
        public string Departement { get; set; }
        public string Klant { get; set; }
        [Display(Name = "Uurregistratie")]
        public string UurRegistratieTitel { get; set; }
        public UurRegistratie UurRegistratie { get; set; }
        public bool Gefactureerd { get; set; }

        public UurRegistratieDetailViewModel()
        {

        }

        public UurRegistratieDetailViewModel(string gebruiker, string klant, string departement, UurRegistratie uurRegistratie, bool gefactureerd)
        {
            Gebruiker = gebruiker;
            Klant = klant;
            Departement = departement;
            UurRegistratie = uurRegistratie;
            UurRegistratieTitel = uurRegistratie.Titel;
            Gefactureerd = gefactureerd;  
        }
    }

    public class FactuurViewModel
    {
        public Factuur Factuur { get; set; }
        public Klant Klant { get; set; }
        public Departement Departement { get; set; }
        public List<FactuurDetail> FactuurDetails { get; set; }
        public bool TeBevestigen { get; set; }
    }

    public class FactuurIndexViewModel
    {
        private IEnumerable<DepartementKlant> departementen = new List<DepartementKlant>();

        [Display(Name = "Departement")]
        public DepartementKlant GekozenDepartement { get; set; }
        public IEnumerable<SelectListItem> Departementen
        {
            get
            {
                return new SelectList(departementen, "Id", "Omschrijving");
            }
        }

        public void setDepartementen(IEnumerable<DepartementKlant> departementen)
        {
            this.departementen = departementen;
        }
    }

}
