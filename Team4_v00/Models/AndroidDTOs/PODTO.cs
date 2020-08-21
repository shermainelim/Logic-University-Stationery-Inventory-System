using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models.AndroidDTOs
{
    public class PODTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ReceiveDate { get; set; }
        public POStatus POStatus { get; set; }
        public string SupplierName { get; set; }
        
    }
}
