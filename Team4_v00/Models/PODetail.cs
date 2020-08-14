using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class PODetail
    {
        public int Id { get; set; }
        public virtual PO PO { get; set; }
        public virtual SupplierDetail SupplierDetail { get; set; }
        public int Qty { get; set; }
        public double Amount => Qty * SupplierDetail.UnitPrice;

    }
}
