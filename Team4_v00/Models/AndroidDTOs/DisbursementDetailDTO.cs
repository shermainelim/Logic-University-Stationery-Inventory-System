using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models.AndroidDTOs
{
    public class DisbursementDetailDTO
    {
        public int Id { get; set; }
        public int StationeryId { get; set; }
        public string StationeryCode { get; set; }
        public string StationeryName { get; set; }
        public int Qty { get; set; }

        [DataType(DataType.Date)]
        public DateTime? A_Date { get; set; }

        public int DisbursementId { get; set; }

        public string DeptName { get; set; }

    }
}
