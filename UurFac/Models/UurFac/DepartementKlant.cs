using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UurFac.Models.UurFac
{
    public class DepartementKlant
    {
        public int Id { get; set; }

        public int DepartementId { get; set; }
        public virtual Departement Departement { get; set; }
        public int KlantId { get; set; }
        public virtual Klant Klant { get; set; }
        
        public virtual ICollection<Factuur> Facturen { get; set; }
        public virtual ISet<GebruikerKlant> GebruikerKlanten { get; set; }

        protected DepartementKlant()
        {
            Facturen = new List<Factuur>();
            GebruikerKlanten = new HashSet<GebruikerKlant>();
        }

        private DepartementKlant(Departement departement, Klant klant) : this()
        {
            Departement = departement;
            DepartementId = departement.Id;
            Klant = klant;
            KlantId = klant.Id;
        }

        public static DepartementKlant koppelDepartementAanKlant(Departement departement, Klant klant)
        {
            DepartementKlant departementKlant = new DepartementKlant(departement, klant);
            klant.DepartementKlanten.Add(departementKlant);
            departement.DepartementKlanten.Add(departementKlant);
            return departementKlant;
        }

        public void ontkoppel()
        {
            if (Facturen.Count > 0)
            {
                throw new Exception("De koppeling Departement - Klant kan niet verwijderd worden omdat er voor deze koppeling Facturen bestaan.");
            }
            Departement.DepartementKlanten.Remove(this);
            Klant.DepartementKlanten.Remove(this);
        }

        public bool Verwijderbaar()
        {
            bool verwijderbaar = Facturen.Count == 0;
            IEnumerator<GebruikerKlant> enumerator = GebruikerKlanten.GetEnumerator();

            while(verwijderbaar && enumerator.MoveNext())
            {
                verwijderbaar = enumerator.Current.Verwijderbaar();
            }

            return verwijderbaar;            
        }

        public IEnumerable<GebruikerDepartement> GebruikerDepartementen()
        {
            return GebruikerKlanten.Select(gk => gk.GebruikerDepartement);
        }

        public override string ToString()
        {
            return String.Format("DepartementKlant - ID={0} - {1} <-> {2}", Id, Klant, Departement);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            DepartementKlant other = (DepartementKlant)obj;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id * 31;
        }
    }
}