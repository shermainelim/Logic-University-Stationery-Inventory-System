using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class AdjustmentVoucher
    {
        public int Id { get; set; }
        public string VoucherNo { get; set; }
        public DateTime IssueDate { get; set; }
        public AdjustmentVoucherStatus Status { get; set; }
        public DeptRole AuthorizedBy { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual List<AdjustmentDetail> AdjustmentDetails { get; set; }
    }
}
