using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models.AndroidDTOs
{
    public class DisbursementDTO
    {
        public int Id { get; set; }
        public string AcknowledgementCode { get; set; }
        public DisbursementStatus DisbursementStatus { get; set; }
        public List<DisbursementDetailDTO> DisbursementDetails { get; set; }
    }
}
