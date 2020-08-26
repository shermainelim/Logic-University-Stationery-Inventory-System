using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        [Index(IsUnique = true)]
        public string Username { get; set; }
        public string Password { get; set; }
        public DeptRole JobTitle { get; set; }
        public DeptRole Role { get; set; }
        public virtual Department Dept { get; set; }
        public virtual List<DelegatedEmployee> DelegatedEmployees { get; set; }
    }
}
