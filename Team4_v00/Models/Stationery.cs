using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class Stationery
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }

        public int ReorderLevel { get; set; }

        public UOM Uom { get; set; }

    }

}
