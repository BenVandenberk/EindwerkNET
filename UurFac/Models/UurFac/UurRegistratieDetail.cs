using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UurFac.Models.UurFac
{
    public class UurRegistratieDetail
    {
        public int Id { get; set; }
        public DateTime StartTijd { get; set; }
        public DateTime EindTijd { get; set; }
        public bool TeFactureren { get; set; }

        public int UurRegistratieId { get; set; }
        public virtual UurRegistratie UurRegistratie { get; set; }
        public int TariefId { get; set; }
        public virtual Tarief Tarief { get; set; }

        public bool isGefactureerd()
        {
            return UurRegistratie.isGefactureerd();
        }

        public decimal Waarde()
        {
            TimeSpan duur = EindTijd.Subtract(StartTijd);
            double inMinuten = duur.TotalMinutes;
            inMinuten -= inMinuten % 15;
            double inUren = inMinuten / 60.0;
            return (decimal)inUren * Tarief.TariefUurWaarde;
        }

        public override string ToString()
        {
            return String.Format("UurRegistratieDetail {0} - Van {1} tot {2} - {3}te factureren", Id, StartTijd, EindTijd, TeFactureren ? "" : "niet ");
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            UurRegistratieDetail other = (UurRegistratieDetail)obj;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id * 31;
        }
    }
}