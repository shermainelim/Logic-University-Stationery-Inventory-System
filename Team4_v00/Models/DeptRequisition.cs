using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class DeptRequisition
    {
        public int Id { get; set; }
        public virtual Employee Employee { get; set; }
        public RequisitionApprovalStatus RequisitionApprovalStatus { get; set; }
        public RequisitionFulfillmentStatus RequisitionFulfillmentStatus { get; set; }
        public virtual List<RequisitionDetail> RequisitionDetails { get; set; }
    }
}
