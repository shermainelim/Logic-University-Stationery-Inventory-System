using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class UsageHistory
    {
        //public int Id { get; set; }

        ////public string Department { get; set; }

        //public virtual Department Department { get; set;r }
        //public int Qty { get; set; }
        ////public virtual Stationery Stationery { get; set; }
        ////public virtual DisbursementDetail DisbursementDetail { get; set; }
        ///
        public int Id { get; set; }
        public virtual Stationery Stationery { get; set; }
        public virtual Department Department { get; set; }
        public int Qty { get; set; }
        public virtual DisbursementDetail DisbursementDetail { get; set; }
    }
    
}
