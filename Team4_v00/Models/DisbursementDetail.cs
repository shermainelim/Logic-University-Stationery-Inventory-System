using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class DisbursementDetail
    {
        public int Id { get; set; }
        public virtual Stationery Stationery { get; set; }
        public int Qty { get; set; }
        public virtual Disbursement Disbursement { get; set; }

           [DataType(DataType.Date)]
        public DateTime? A_Date { get; set; }
        public virtual Department Department { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
