using Ben_Project.Models;
using Castle.Core.Configuration;
using Microsoft.EntityFrameworkCore;
using System;

namespace Ben_Project.DB
{
    public class LogicContext : DbContext
    {
        protected IConfiguration configuration;

        public DbSet<Stationery> Stationeries { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DeptRequisition> DeptRequisitions { get; set; }
        public DbSet<RequisitionDetail> RequisitionDetails { get; set; }
        public DbSet<Disbursement> Disbursements { get; set; }
        public DbSet<DisbursementDetail> DisbursementDetails { get; set; }
        public DbSet<AdjustmentVoucher> AdjustmentVouchers { get; set; }
        public DbSet<AdjustmentDetail> AdjustmentDetails { get; set; }
        public DbSet<PO> POs { get; set; }
        public DbSet<PODetail> PODetails { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierDetail> SupplierDetails { get; set; }
        public DbSet<UsageHistory> UsageHistories { get; set; }
        public DbSet<DelegatedEmployee> DelegatedEmployees { get; set; }
        public DbSet<DelegateEmployeeDetail> DelegateEmployeeDetails { get; set; }


        public LogicContext(DbContextOptions<LogicContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
        }

    }
}
