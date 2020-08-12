using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class RequisitionDetail
    {
        public int Id { get; set; }
        public virtual Stationery Stationery { get; set; }
        public int Qty { get; set; }
        public int CollectedQty { get; set; }
        public virtual DeptRequisition DeptRequisition { get; set; }
    }
}
