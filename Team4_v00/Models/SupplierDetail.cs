using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class SupplierDetail
    {
        public int Id { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual Stationery Stationery { get; set; }
        public double UnitPrice { get; set; }
    }
}
