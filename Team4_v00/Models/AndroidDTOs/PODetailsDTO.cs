using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models.AndroidDTOs
{
    public class PODetailsDTO
    {
        public int Id { get; set; }

        public int poID { get; set; }

        public Stationery stationery { get; set; }

        public double unitPrice { get; set; }

        public double predictionQty { get; set; }

        public int Qty { get; set; }
    }
}
