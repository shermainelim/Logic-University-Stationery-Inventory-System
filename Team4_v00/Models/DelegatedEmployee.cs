using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class DelegatedEmployee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DelegationStatus delegationStatus { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual List<DelegateEmployeeDetail> DelegateEmployeeDetails { get; set; }

    }
}
