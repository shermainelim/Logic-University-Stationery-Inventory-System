using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class Disbursement
    {
        public int Id { get; set; }
        public DateTime? DisbursementDate { get; set; }
        public string AcknowledgementCode { get; set; }
        public DisbursementStatus DisbursementStatus { get; set; }
        public virtual DeptRequisition DeptRequisition { get; set; }
        public virtual List<DisbursementDetail> DisbursementDetails { get; set; }

    }
}
