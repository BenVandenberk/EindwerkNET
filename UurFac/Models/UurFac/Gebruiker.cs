using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UurFac.Models.UurFac
{
    public enum Rol {
        ADMINISTRATOR, DEPARTEMENT_ADMINISTRATOR, USER, BEZOEKER           
    };

    public class Gebruiker
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string Gsm { get; set; }
        public Rol Rol { get; set; }

        [Required]
        public virtual ApplicationUser User { get; set; }

        public virtual ISet<GebruikerDepartement> GebruikerDepartementen { get; set; }        

        public Gebruiker()
        {
            GebruikerDepartementen = new HashSet<GebruikerDepartement>();            
        }

        public IEnumerable<Departement> Departementen()
        {
            return GebruikerDepartementen.Select(gd => gd.Departement);
        }        

        public string volledigeNaam()
        {
            return String.Format("{0} {1}", Voornaam, Achternaam);
        }

        public bool Verwijderbaar()
        {
            bool verwijderbaar = true;

            IEnumerator<GebruikerDepartement> enumerator = GebruikerDepartementen.GetEnumerator();

            while (verwijderbaar && enumerator.MoveNext())
            {
                verwijderbaar = enumerator.Current.Verwijderbaar();
            }

            return verwijderbaar;
        }

        public GebruikerDepartement ontkoppelVanDepartement(Departement departement)
        {
            GebruikerDepartement koppeling = GebruikerDepartementen.SingleOrDefault(gd => gd.Departement.Equals(departement));

            if (koppeling != null)
            {
                koppeling.ontkoppel();
            }
            return koppeling;
        }      

        public List<GebruikerKlant> GebruikerKlanten()
        {
            List<GebruikerKlant> result = new List<GebruikerKlant>();

            foreach(GebruikerDepartement gd in GebruikerDepartementen)
            {
                result.AddRange(gd.GebruikerKlanten);
            }

            return result;
        }

        public string rolString()
        {
            switch (Rol)
            {
                case Rol.ADMINISTRATOR:
                    return "Administrator";
                case Rol.DEPARTEMENT_ADMINISTRATOR:
                    return "Departement Administrator";
                        case Rol.USER:
                    return "User";
                default:
                    return "";
            }
        }

        public override string ToString()
        {
            return String.Format("Gebruiker {0} - {1} - {2} {3}", Id, Login, Voornaam, Achternaam);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Gebruiker other = (Gebruiker)obj;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id * 31;
        }
    }

   
}