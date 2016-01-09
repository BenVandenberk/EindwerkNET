using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UurFac.Models.UurFac
{
    public class Factuur
    {
        public int Id { get; set; }
        public int FactuurJaar { get; set; }
        public string FactuurNummer { get; set; }
        public DateTime FactuurDatum { get; set; }
        public decimal Totaal { get; set; }

        public int DepartementKlantId { get; set; }
        public virtual DepartementKlant DepartementKlant { get; set; }

        public virtual ISet<FactuurDetail> FactuurDetails { get; set; }

        public Factuur()
        {
            FactuurDetails = new HashSet<FactuurDetail>();
        }

        public void BerekenTotaal()
        {
            Totaal = 0.0M;

            foreach (FactuurDetail facDetail in FactuurDetails)
            {
                Totaal += facDetail.LijnWaarde;
            }
        }

        public override string ToString()
        {
            return String.Format("Factuur {0} - Jaar: {1} - Nummer: {2}", Id, FactuurJaar, FactuurNummer);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Factuur other = (Factuur)obj;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id * 31;
        }
    }
}