using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ben_Project.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ben_Project.ViewModels
{
    public class RequisitionViewModel
    {
        public DeptRequisition DeptRequisition { get; set; }
        public RequisitionApprovalStatus RequisitionApprovalStatus { get; set; }
        public IEnumerable<SelectListItem> RequisitionApprovalStatuses { get; set; }

        public RequisitionViewModel()
        {
            RequisitionApprovalStatuses = Enum.GetNames(typeof(RequisitionApprovalStatus)).Select(name =>
                new SelectListItem()
                {
                    Text = name,
                    Value = name
                });
        }
    }
}
