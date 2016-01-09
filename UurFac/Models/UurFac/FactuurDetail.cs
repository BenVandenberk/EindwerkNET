using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UurFac.Models.UurFac
{
    public class FactuurDetail
    {
        public int Id { get; set; }
        public string Omschrijving { get; set; }
        public decimal LijnWaarde { get; set; }

        public int FactuurId { get; set; }
        public virtual Factuur Factuur { get; set; }

        public int UurRegistratieId { get; set; }
        public virtual UurRegistratie UurRegistratie { get; set; }

        public void BerekenLijnWaarde()
        {
            decimal waarde = 0.0M;

            foreach (UurRegistratieDetail uurDetail in UurRegistratie.UurRegistratieDetails)
            {
                if (uurDetail.TeFactureren)
                {
                    waarde += uurDetail.Waarde();
                }
            }

            LijnWaarde = waarde;
        }

        public override string ToString()
        {
            return String.Format("FactuurDetail {0} - {1} - Waarde: {2}", Id, Omschrijving, LijnWaarde);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            FactuurDetail other = (FactuurDetail)obj;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id * 31;
        }
    }
}