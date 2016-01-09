using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UurFac.Models.UurFac
{
    public class Departement
    {
        public int Id { get; set; }        
        public string Code { get; set; }
        public string Omschrijving { get; set; }

        public virtual ISet<GebruikerDepartement> GebruikerDepartementen { get; set; }
        public virtual ISet<DepartementKlant> DepartementKlanten { get; set; }

        public Departement()
        {
            GebruikerDepartementen = new HashSet<GebruikerDepartement>();
            DepartementKlanten = new HashSet<DepartementKlant>();
        }

        public IEnumerable<Klant> Klanten()
        {
            return DepartementKlanten.Select(dk => dk.Klant);
        }

        public IEnumerable<Gebruiker> Gebruikers()
        {
            return GebruikerDepartementen.Select(gd => gd.Gebruiker);
        }

        public bool Verwijderbaar()
        {
            return GebruikerDepartementen.Count == 0 && DepartementKlanten.Count == 0;
        }

        public override string ToString()
        {
            return Omschrijving;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Departement other = (Departement)obj;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id * 31;
        }
    }
}