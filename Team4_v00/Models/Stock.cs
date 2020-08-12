using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public virtual Stationery Stationery { get; set; }
        public int Qty { get; set; }
        public int ReorderLevel { get; set; }
        public int ReorderQty { get; set; }
        public int Forecast { get; set; }
    }
}
