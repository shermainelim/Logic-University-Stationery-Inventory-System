using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models.AndroidDTOs
{
    //Summer add
    public class ResponsePOCreateStep1
    {
        public int PoId { get; set; }
        public List<string> ItemNames { get; set; }
        public List<double> UnitPrices { get; set; }
    }
}
