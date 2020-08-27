using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ben_Project.Models;
using Microsoft.AspNetCore.Http;

namespace Ben_Project.Services.UserRoleFilterService
{
    public class UserRoleFilterService
    {
        public string Filter(string role)
        {  
            if (role.Equals(DeptRole.DeptHead.ToString()))
            {
                return "DeptHeadRequisitionList";
            }
            else if (role.Equals(DeptRole.DeptRep.ToString()))
            {
                return "EmployeeRequisitionList";
            }
            else if (role.Equals(DeptRole.DelegatedEmployee.ToString()))
            {
                return "DeptHeadRequisitionList";
            }
            else if (role.Equals(DeptRole.Employee.ToString()) || role.Equals(DeptRole.Contact.ToString()))
            {
                return "EmployeeRequisitionList";
            }
            else if (role.Equals(DeptRole.StoreClerk.ToString()))
            {
                return "BarChart";
            }
            else if (role.Equals(DeptRole.StoreManager.ToString()) || role.Equals(DeptRole.StoreSupervisor.ToString()))
            {
                return "AuthorizeAdjustmentVoucherList";
            }
            return null;
        }
    }
}
