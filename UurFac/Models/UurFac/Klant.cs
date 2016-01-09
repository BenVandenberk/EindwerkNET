using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UurFac.Models.UurFac
{
    public class Klant
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Het ondernemingsnummer is verplicht")]
        public string Ondernemingsnummer { get; set; }

        [Required(ErrorMessage = "De bedrijfsnaam is verplicht")]
        public string Bedrijfsnaam { get; set; }

        [Required(ErrorMessage = "Het adres is verplicht")]
        public string Adres { get; set; }

        [Required(ErrorMessage = "De postcode is verplicht")]
        public string Postcode { get; set; }

        [Required(ErrorMessage = "De plaats is verplicht")]
        public string Plaats { get; set; }

        public virtual ISet<DepartementKlant> DepartementKlanten { get; set; }        

        public Klant()
        {
            DepartementKlanten = new HashSet<DepartementKlant>();            
        }

        public IEnumerable<Departement> Departementen()
        {
            return DepartementKlanten.Select(dk => dk.Departement);
        }       

        public DepartementKlant OntkoppelVanDepartement(Departement departement)
        {
            DepartementKlant koppeling = DepartementKlanten.SingleOrDefault(dk => dk.Departement.Equals(departement));
            koppeling.ontkoppel();
            return koppeling;
        }

        public List<GebruikerKlant> GebruikerKlanten()
        {
            List<GebruikerKlant> result = new List<GebruikerKlant>();

            foreach(DepartementKlant dk in DepartementKlanten)
            {
                result.AddRange(dk.GebruikerKlanten);
            }

            return result;
        }

        public bool Verwijderbaar()
        {
            bool verwijderbaar = true;

            IEnumerator<DepartementKlant> enumerator = DepartementKlanten.GetEnumerator();            

            while(verwijderbaar && enumerator.MoveNext())
            {
                verwijderbaar = enumerator.Current.Verwijderbaar();
            }

            return verwijderbaar;
        }

        public override string ToString()
        {
            return Bedrijfsnaam;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Klant other = (Klant)obj;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id * 31;
        }
    }
}