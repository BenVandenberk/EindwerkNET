using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UurFac.Models.UurFac
{
    public class GebruikerKlant
    {
        public int Id { get; set; }

        public int GebruikerDepartementId { get; set; }
        public virtual GebruikerDepartement GebruikerDepartement { get; set; } 
        public int DepartementKlantId { get; set; }
        public virtual DepartementKlant DepartementKlant { get; set; }

        public virtual ISet<UurRegistratie> UurRegistraties { get; set; }

        protected GebruikerKlant()
        {
            UurRegistraties = new HashSet<UurRegistratie>();
        }

        private GebruikerKlant(GebruikerDepartement gebruikerDepartement, DepartementKlant departementKlant) : this()
        {
            GebruikerDepartement = gebruikerDepartement;
            GebruikerDepartementId = gebruikerDepartement.Id;
            DepartementKlant = departementKlant;
            DepartementKlantId = departementKlant.Id;
        }

        public static GebruikerKlant koppelGebruikerAanKlant(GebruikerDepartement gebruikerDepartement, DepartementKlant departementKlant)
        {
            Departement klantDep = departementKlant.Departement;
            Departement gebruikerDep = gebruikerDepartement.Departement;            

            if (!klantDep.Equals(gebruikerDep))
            {
                throw new Exception("De gebruiker kan niet gekoppeld worden aan de klant. De gebruiker behoort tot geen enkel departement waar de klant aan gekoppeld is");
            }

            GebruikerKlant koppeling = new GebruikerKlant(gebruikerDepartement, departementKlant);
            gebruikerDepartement.GebruikerKlanten.Add(koppeling);
            departementKlant.GebruikerKlanten.Add(koppeling);

            return koppeling;
        }

        public bool Verwijderbaar()
        {
            return UurRegistraties.Count == 0;
        }

        public void ontkoppel()
        {
            if (UurRegistraties.Count > 0)
            {
                throw new Exception("Aan de koppeling Gebruiker - Klant zijn al UurRegistraties gekoppeld");
            }
            GebruikerDepartement.GebruikerKlanten.Remove(this);
            DepartementKlant.GebruikerKlanten.Remove(this);
        }

        public Klant Klant()
        {
            return DepartementKlant.Klant;
        }

        public override string ToString()
        {
            return String.Format("GebruikerKlant - ID={0} - {1} <-> {2}", Id, GebruikerDepartement, DepartementKlant);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            GebruikerKlant other = (GebruikerKlant)obj;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id * 31;
        }
    }
}