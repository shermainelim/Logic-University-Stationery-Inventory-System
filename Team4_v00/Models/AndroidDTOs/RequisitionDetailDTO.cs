using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models.AndroidDTOs
{
    public class RequisitionDetailDTO
    {
        public int Id { get; set; }
        public int StationeryId { get; set; }
        public string StationeryName { get; set; }
        public int Qty { get; set; }
        public int StockQty { get; set; }
        public int CollectedQty { get; set; }
        public int DisbursedQty { get; set; }
    }
}
