using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class PO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public virtual Supplier Supplier { get; set; }

        public virtual List<PODetail> PODetails { get; set; }

        public POStatus POStatus { get; set; }

        




    }
}
