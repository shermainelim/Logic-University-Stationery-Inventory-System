using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class AdjustmentDetail
    {
        public int Id { get; set; }
        public virtual Stationery Stationery { get; set; }
        public int AdjustedQty { get; set; }
        public string Reason { get; set; }
    }
}
