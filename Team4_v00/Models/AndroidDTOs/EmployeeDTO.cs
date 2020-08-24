using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ben_Project.Models.AndroidDTOs
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DeptId { get; set; }
        public DeptRole Role { get; set; }
    }
}
