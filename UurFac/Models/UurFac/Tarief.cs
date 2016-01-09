using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UurFac.Models.UurFac
{
    public class Tarief
    {
        public int Id { get; set; }
        public string TypeWerk { get; set; }
        public DateTime GeldigVanaf { get; set; }
        public decimal TariefUurWaarde { get; set; }

        public override string ToString()
        {
            return String.Format("Tarief {0} - {1} - € {2}/uur", Id, TypeWerk, TariefUurWaarde);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Tarief other = (Tarief)obj;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id * 31;
        }
    }
}