using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class Supplier
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Address { get; set; }
        public string TelephoneNo { get; set; }
        public SupplierStatus supplierStatus { get; set; }
        public virtual List<SupplierDetail> SupplierDetails { get; set; }
    }
}
