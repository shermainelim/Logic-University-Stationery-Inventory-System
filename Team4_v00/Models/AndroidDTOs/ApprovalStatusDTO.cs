using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models.AndroidDTOs
{
    public class ApprovalStatusDTO
    {
        public int Id { get; set; }
        public string RequisitionApprovalStatus { get; set; }
        public string Reason { get; set; }
    }
}
