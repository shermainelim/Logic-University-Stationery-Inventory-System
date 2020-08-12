using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class Department
    {
        public int id { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public CollectionPoint CollectionPoint { get; set; }
    }
}
