using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models.AndroidDTOs
{
    public class DeptRequisitionDTO
    {
        public int Id { get; set; }
        public EmployeeDTO Employee { get; set; }
        public string FormStatus { get; set; }
        public RequisitionApprovalStatus RequisitionApprovalStatus { get; set; }
        public RequisitionFulfillmentStatus RequisitionFulfillmentStatus { get; set; }
        public List<RequisitionDetailDTO> RequisitionDetails { get; set; }

        public DeptRequisitionDTO()
        {
            RequisitionDetails = new List<RequisitionDetailDTO>();
        }
    }
}
