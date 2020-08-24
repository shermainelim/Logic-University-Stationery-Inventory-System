using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class DelegatedEmployees
    {
        public int id { get; set; }
        public string name { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public DelegationStatus status { get; set; }
    }
}
