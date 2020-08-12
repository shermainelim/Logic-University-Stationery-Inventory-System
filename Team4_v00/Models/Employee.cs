using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DeptRole JobTitle { get; set; }
        public DeptRole Role { get; set; }
        public virtual Department Dept { get; set; }
    }
}
