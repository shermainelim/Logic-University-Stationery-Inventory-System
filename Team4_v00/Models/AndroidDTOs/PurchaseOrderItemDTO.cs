using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models.AndroidDTOs
{
    public class PurchaseOrderItemDTO
    {
        public DateTime OrderDate { get; set; }

        public POStatus POStatus { get; set; }

        public int supplierID { get; set; }

        public List<PODetailsDTO> poDetailsList { get; set; }
    }
}
