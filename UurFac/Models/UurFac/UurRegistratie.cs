using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UurFac.Models.UurFac
{
    public class UurRegistratie
    {
        public int Id { get; set; }
        public string Titel { get; set; }
        public string Omschrijving { get; set; }

        public int GebruikerKlantId { get; set; }
        public virtual GebruikerKlant GebruikerKlant { get; set; }
        public int? FactuurDetailId { get; set; }
        public virtual FactuurDetail FactuurDetail { get; set; }
       
        public virtual ISet<UurRegistratieDetail> UurRegistratieDetails { get; set; }

        public UurRegistratie()
        {            
            UurRegistratieDetails = new HashSet<UurRegistratieDetail>();
        }

        public bool isGefactureerd()
        {
            return FactuurDetail != null;
        }

        public bool isInPeriode(DateTime? van, DateTime? tot)
        {
            if (!van.HasValue && !tot.HasValue)
            {
                return true;
            }

            bool isInPeriode = false;
            IEnumerator<UurRegistratieDetail> enumerator = UurRegistratieDetails.GetEnumerator();
            
            while (!isInPeriode && enumerator.MoveNext())
            {
                DateTime start = enumerator.Current.StartTijd;
                if (!van.HasValue)
                {
                    isInPeriode = start.CompareTo(tot.Value) <= 0;
                }
                else if (!tot.HasValue)
                {
                    isInPeriode = start.CompareTo(van.Value) >= 0;
                }
                else
                {
                    if (van.Value.CompareTo(tot.Value) > 0)
                    {
                        throw new InvalidOperationException("Datum 'van' moet eerder zijn dan datum 'tot'");
                    }
                    isInPeriode = start.CompareTo(van.Value) >= 0 && start.CompareTo(tot.Value) <= 0;
                }
            }

            return isInPeriode;
        }

        public override string ToString()
        {
            return String.Format("UurRegistratie {0} - {1} - {2}", Id, Titel, Omschrijving);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            UurRegistratie other = (UurRegistratie)obj;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id * 31;
        }
    }
}