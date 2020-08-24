using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class TempItems
    {
        public int id { get; set; }

        public int supplierId { get; set; }

        public DateTime orderDate { get; set; }
    }
}
