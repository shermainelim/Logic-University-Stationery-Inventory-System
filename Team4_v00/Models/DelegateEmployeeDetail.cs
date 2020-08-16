using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class DelegateEmployeeDetail
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual DelegatedEmployee DelegatedEmployee { get; set; }
    }
}
