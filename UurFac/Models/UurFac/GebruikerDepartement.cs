using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UurFac.Models.UurFac
{
    public class GebruikerDepartement
    {
        public int Id { get; set; }

        public int GebruikerId { get; set; }        
        public virtual Gebruiker Gebruiker { get; set; }
        public int DepartementId { get; set; }        
        public virtual Departement Departement { get; set; }

        public virtual ISet<GebruikerKlant> GebruikerKlanten { get; set; }

        protected GebruikerDepartement()
        {
            GebruikerKlanten = new HashSet<GebruikerKlant>();
        }

        private GebruikerDepartement(Gebruiker gebruiker, Departement departement) {
            Gebruiker = gebruiker;
            GebruikerId = gebruiker.Id;
            Departement = departement;
            DepartementId = departement.Id;
        }

        public static GebruikerDepartement koppelGebruikerAanDepartement(Gebruiker gebruiker, Departement departement)
        {
            GebruikerDepartement gebruikerDepartement = new GebruikerDepartement(gebruiker, departement);
            gebruiker.GebruikerDepartementen.Add(gebruikerDepartement);
            departement.GebruikerDepartementen.Add(gebruikerDepartement);
            return gebruikerDepartement;
        }        

        public bool Verwijderbaar()
        {
            bool verwijderbaar = true;

            IEnumerator<GebruikerKlant> enumerator = GebruikerKlanten.GetEnumerator();

            while(verwijderbaar && enumerator.MoveNext())
            {
                verwijderbaar = enumerator.Current.Verwijderbaar();
            }

            return verwijderbaar;
        }

        public void ontkoppel()
        {
            Gebruiker.GebruikerDepartementen.Remove(this);
            Departement.GebruikerDepartementen.Remove(this);
        }

        public IEnumerable<DepartementKlant> DepartementKlanten()
        {
            return GebruikerKlanten.Select(gk => gk.DepartementKlant);
        }

        public IEnumerable<Klant> Klanten()
        {
            return GebruikerKlanten.Select(gk => gk.DepartementKlant.Klant);
        }

        public override string ToString()
        {
            return String.Format("GebruikerDepartement - ID={0} - {1} <-> {2}", Id, Gebruiker, Departement);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            GebruikerDepartement other = (GebruikerDepartement)obj;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id * 31;
        }

    }
}