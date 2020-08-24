using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Ben_Project.Models;
using Ben_Project.Utils;
using Microsoft.EntityFrameworkCore;

namespace Ben_Project.DB
{
    public class DBSeeder
    {
        public DBSeeder(LogicContext dbContext)
        {
            // Adding Stationery Data


            Stationery s1 = new Stationery();
            s1.ItemNumber = "C001";
            s1.Category = Category.Clip;
            s1.Description = "Clips Double 1";
            s1.ReorderLevel = 50;
            s1.Uom = UOM.Dozen;

            Stationery s2 = new Stationery();
            s2.ItemNumber = "C004";
            s2.Category = Category.Clip;
            s2.ReorderLevel = 50;
            s2.Description = "Clips Paper Large";

            s2.Uom = UOM.Box;

            Stationery s3 = new Stationery();
            s3.ItemNumber = "E001";
            s3.Category = Category.Envelope;
            s3.ReorderLevel = 600;
            s3.Description = "Envelope Brown (3\"x6\")";

            s3.Uom = UOM.Each;

            Stationery s4 = new Stationery();
            s4.ItemNumber = "E020";
            s4.Category = Category.Eraser;
            s4.ReorderLevel = 50;
            s4.Description = "Eraser (hard)";

            s4.Uom = UOM.Each;

            Stationery s5 = new Stationery();
            s5.ItemNumber = "E030";
            s5.Category = Category.Exercise;
            s5.ReorderLevel = 100;
            s5.Description = "Exercise Book (100 pg)";

            s5.Uom = UOM.Each;

            Stationery s6 = new Stationery();
            s6.ItemNumber = "F020";
            s6.Category = Category.File;
            s6.ReorderLevel = 100;
            s6.Description = "File Separator";

            s6.Uom = UOM.Set;

            Stationery s7 = new Stationery();
            s7.ItemNumber = "F021";
            s7.Category = Category.File;
            s7.Description = "File-Blue Plain";
            s7.ReorderLevel = 200;
            s7.Uom = UOM.Each;

            Stationery s8 = new Stationery();
            s8.ItemNumber = "H011";
            s8.Category = Category.Pen;
            s8.Description = "Highlighter Blue";
            s8.ReorderLevel = 100;
            s8.Uom = UOM.Box;

            Stationery s9 = new Stationery();
            s9.ItemNumber = "H031";
            s9.Category = Category.Puncher;
            s9.Description = "Hole Puncher 2 holes";
            s9.ReorderLevel = 50;
            s9.Uom = UOM.Each;

            Stationery s10 = new Stationery();
            s10.ItemNumber = "P010";
            s10.Category = Category.Pad;
            s10.Description = "Pad Postit Memo 1\"x2\"";
            s10.ReorderLevel = 100;
            s10.Uom = UOM.Packet;

            Stationery s11 = new Stationery();
            s11.ItemNumber = "P020";
            s11.Category = Category.Paper;
            s11.Description = "Paper Photostat A3";
            s11.ReorderLevel = 500;
            s11.Uom = UOM.Box;

            Stationery s12 = new Stationery();
            s12.ItemNumber = "P030";
            s12.Category = Category.Pen;
            s12.Description = "Pen Ballpoint Black";
            s12.ReorderLevel = 100;
            s12.Uom = UOM.Dozen;

            Stationery s13 = new Stationery();
            s13.ItemNumber = "P036";
            s13.Category = Category.Pen;
            s13.Description = "Pen Transparency Permanent";
            s13.ReorderLevel = 100;
            s13.Uom = UOM.Packet;

            Stationery s14 = new Stationery();
            s14.ItemNumber = "P038";
            s14.Category = Category.Pen;
            s14.Description = "Pen Whiteboard Marker Black";
            s14.ReorderLevel = 100;

            s14.Uom = UOM.Box;

            Stationery s15 = new Stationery();
            s15.ItemNumber = "P042";
            s15.Category = Category.Pen;
            s15.ReorderLevel = 100;
            s15.Description = "Pencil 2B";

            s15.Uom = UOM.Dozen;

            Stationery s16 = new Stationery();
            s16.ItemNumber = "R002";
            s16.Category = Category.Ruler;
            s16.ReorderLevel = 50;
            s16.Description = "Ruler 12\"";

            s16.Uom = UOM.Dozen;

            Stationery s17 = new Stationery();
            s17.ItemNumber = "S100";
            s17.Category = Category.Scissors;
            s17.ReorderLevel = 50;
            s17.Description = "Scissors";

            s17.Uom = UOM.Each;

            Stationery s18 = new Stationery();
            s18.ItemNumber = "S040";
            s18.Category = Category.Tape;
            s18.Description = "Scotch Tape";
            s18.ReorderLevel = 50;
            s18.Uom = UOM.Each;

            Stationery s19 = new Stationery();
            s19.ItemNumber = "S101";
            s19.Category = Category.Sharpener;
            s19.Description = "Sharpener";
            s19.ReorderLevel = 50;
            s19.Uom = UOM.Each;

            Stationery s20 = new Stationery();
            s20.ItemNumber = "S010";
            s20.Category = Category.Shorthand;
            s20.Description = "Shorthand Book (100 pg)";
            s20.ReorderLevel = 100;
            s20.Uom = UOM.Each;

            Stationery s21 = new Stationery();
            s21.ItemNumber = "S020";
            s21.Category = Category.Stapler;
            s21.Description = "Stapler No. 28";
            s21.ReorderLevel = 50;
            s21.Uom = UOM.Each;

            Stationery s22 = new Stationery();
            s22.ItemNumber = "T001";
            s22.Category = Category.Tacks;
            s22.Description = "Thumb Tacks Large";
            s22.ReorderLevel = 10;
            s22.Uom = UOM.Box;

            Stationery s23 = new Stationery();
            s23.ItemNumber = "T020";
            s23.Category = Category.Tparency;
            s23.Description = "Transparency Blue";
            s23.ReorderLevel = 100;
            s23.Uom = UOM.Box;

            Stationery s24 = new Stationery();
            s24.ItemNumber = "T100";
            s24.Category = Category.Tray;
            s24.Description = "Trays In/Out";
            s24.ReorderLevel = 20;
            s24.Uom = UOM.Set;

            dbContext.Add(s1);
            dbContext.Add(s2);
            dbContext.Add(s3);
            dbContext.Add(s4);
            dbContext.Add(s5);
            dbContext.Add(s6);
            dbContext.Add(s7);
            dbContext.Add(s8);
            dbContext.Add(s9);
            dbContext.Add(s10);
            dbContext.Add(s11);
            dbContext.Add(s12);
            dbContext.Add(s13);
            dbContext.Add(s14);
            dbContext.Add(s15);
            dbContext.Add(s16);
            dbContext.Add(s17);
            dbContext.Add(s18);
            dbContext.Add(s19);
            dbContext.Add(s20);
            dbContext.Add(s21);
            dbContext.Add(s22);
            dbContext.Add(s23);
            dbContext.Add(s24);

            // Adding Stock

            Stock st1 = new Stock();
            st1.Stationery = s1;
            st1.ReorderLevel = 20;
            st1.ReorderQty = 30;
            st1.Qty = 1000;
            st1.UnitPrice = 50.0;

            Stock st2 = new Stock();
            st2.Stationery = s2;
            st2.ReorderLevel = 20;
            st2.ReorderQty = 30;
            st2.Qty = 100;
            st2.UnitPrice = 50.0;

            Stock st3 = new Stock();
            st3.Stationery = s3;
            st3.ReorderLevel = 20;
            st3.ReorderQty = 30;
            st3.Qty = 100;
            st3.UnitPrice = 50.0;

            Stock st4 = new Stock();
            st4.Stationery = s4;
            st4.ReorderLevel = 20;
            st4.ReorderQty = 30;
            st4.Qty = 100;
            st4.UnitPrice = 50.0;

            Stock st5 = new Stock();
            st5.Stationery = s5;
            st5.ReorderLevel = 20;
            st5.ReorderQty = 30;
            st5.Qty = 100;
            st5.UnitPrice = 50.0;

            Stock st6 = new Stock();
            st6.Stationery = s6;
            st6.ReorderLevel = 20;
            st6.ReorderQty = 30;
            st6.Qty = 100;
            st6.UnitPrice = 50.0;

            Stock st7 = new Stock();
            st7.Stationery = s7;
            st7.ReorderLevel = 20;
            st7.ReorderQty = 30;
            st7.Qty = 100;
            st7.UnitPrice = 50.0;

            Stock st8 = new Stock();
            st8.Stationery = s8;
            st8.ReorderLevel = 20;
            st8.ReorderQty = 30;
            st8.Qty = 100;
            st8.UnitPrice = 50.0;

            Stock st9 = new Stock();
            st9.Stationery = s9;
            st9.ReorderLevel = 20;
            st9.ReorderQty = 30;
            st9.Qty = 100;
            st9.UnitPrice = 50.0;

            Stock st10 = new Stock();
            st10.Stationery = s10;
            st10.ReorderLevel = 20;
            st10.ReorderQty = 30;
            st10.Qty = 100;
            st10.UnitPrice = 50.0;

            Stock st11 = new Stock();
            st11.Stationery = s11;
            st11.ReorderLevel = 20;
            st11.ReorderQty = 30;
            st11.Qty = 100;
            st11.UnitPrice = 50.0;

            Stock st12 = new Stock();
            st12.Stationery = s12;
            st12.Qty = 100;
            st12.ReorderLevel = 20;
            st12.ReorderQty = 30;
            st12.UnitPrice = 50.0;

            Stock st13 = new Stock();
            st13.Stationery = s13;
            st13.Qty = 100;
            st13.ReorderLevel = 20;
            st13.ReorderQty = 30;
            st13.UnitPrice = 50.0;

            Stock st14 = new Stock();
            st14.Stationery = s14;
            st14.Qty = 100;
            st14.ReorderLevel = 20;
            st14.ReorderQty = 30;
            st14.UnitPrice = 50.0;

            Stock st15 = new Stock();
            st15.Stationery = s15;
            st15.Qty = 100;
            st15.ReorderLevel = 20;
            st15.ReorderQty = 30;
            st15.UnitPrice = 50.0;

            Stock st16 = new Stock();
            st16.Stationery = s16;
            st16.Qty = 100;
            st16.ReorderLevel = 20;
            st16.ReorderQty = 30;
            st16.UnitPrice = 50.0;

            Stock st17 = new Stock();
            st17.Stationery = s17;
            st17.Qty = 100;
            st17.ReorderLevel = 20;
            st17.ReorderQty = 30;
            st17.UnitPrice = 50.0;

            Stock st18 = new Stock();
            st18.Stationery = s18;
            st18.Qty = 100;
            st18.ReorderLevel = 20;
            st18.ReorderQty = 30;
            st18.UnitPrice = 50.0;

            Stock st19 = new Stock();
            st19.Stationery = s19;
            st19.Qty = 100;
            st19.ReorderLevel = 20;
            st19.ReorderQty = 30;
            st19.UnitPrice = 50.0;

            Stock st20 = new Stock();
            st20.Stationery = s20;
            st20.Qty = 100;
            st20.ReorderLevel = 20;
            st20.ReorderQty = 30;
            st20.UnitPrice = 50.0;

            Stock st21 = new Stock();
            st21.Stationery = s21;
            st21.Qty = 100;
            st21.ReorderLevel = 20;
            st21.ReorderQty = 30;
            st21.UnitPrice = 50.0;

            Stock st22 = new Stock();
            st22.Stationery = s22;
            st22.Qty = 100;
            st22.ReorderLevel = 20;
            st22.ReorderQty = 30;
            st22.UnitPrice = 50.0;

            Stock st23 = new Stock();
            st23.Stationery = s23;
            st23.Qty = 100;
            st23.ReorderLevel = 20;
            st23.ReorderQty = 30;
            st23.UnitPrice = 50.0;

            Stock st24 = new Stock();
            st24.Stationery = s24;
            st24.Qty = 100;
            st24.ReorderLevel = 20;
            st24.ReorderQty = 30;
            st24.UnitPrice = 50.0;

            dbContext.Add(st1);
            dbContext.Add(st2);
            dbContext.Add(st3);
            dbContext.Add(st4);
            dbContext.Add(st5);
            dbContext.Add(st6);
            dbContext.Add(st7);
            dbContext.Add(st8);
            dbContext.Add(st9);
            dbContext.Add(st10);
            dbContext.Add(st11);
            dbContext.Add(st12);
            dbContext.Add(st13);
            dbContext.Add(st14);
            dbContext.Add(st15);
            dbContext.Add(st16);
            dbContext.Add(st17);
            dbContext.Add(st18);
            dbContext.Add(st19);
            dbContext.Add(st20);
            dbContext.Add(st21);
            dbContext.Add(st22);
            dbContext.Add(st23);
            dbContext.Add(st24);

            // Adding Departments
            Department d1 = new Department();
            d1.DeptCode = "ENGL";
            d1.DeptName = "English Dept";
            d1.TelephoneNo = "874 2234";
            d1.FaxNo = "892 1456";
            d1.CollectionPoint = CollectionPoint.EngineeringSchool;

            Department d2 = new Department();
            d2.DeptCode = "CPSC";
            d2.DeptName = "Computer Science";
            d2.TelephoneNo = "890 1235";
            d2.FaxNo = "892 1457";
            d2.CollectionPoint = CollectionPoint.ManagementSchool;

            Department d3 = new Department();
            d3.DeptCode = "COMM";
            d3.DeptName = "Commerce Dept";
            d3.TelephoneNo = "874 1284";
            d3.FaxNo = "892 1256";
            d3.CollectionPoint = CollectionPoint.MedicalSchool;

            Department d4 = new Department();
            d4.DeptCode = "REGR";
            d4.DeptName = "Registrar Dept";
            d4.TelephoneNo = "890 1266";
            d4.FaxNo = "892 1465";
            d4.CollectionPoint = CollectionPoint.ScienceSchool;

            Department d5 = new Department();
            d5.DeptCode = "STORE";
            d5.DeptName = "Store Dept";
            d5.TelephoneNo = "890 1266";
            d5.FaxNo = "892 1465";
            d5.CollectionPoint = CollectionPoint.StationeryStore;

            dbContext.Add(d1);
            dbContext.Add(d2);
            dbContext.Add(d3);
            dbContext.Add(d4);
            dbContext.Add(d5);


            // Adding Employees

            Employee e1 = new Employee();
            e1.Name = "Jenny Wong Mei Lin";
            e1.Username = "employee1";
            e1.Password = Crypto.Sha256("employee1");
            e1.Dept = d3;
            e1.JobTitle = DeptRole.Employee;
            e1.Role = DeptRole.DeptRep;

            Employee e2 = new Employee();
            e2.Name = "Mrs Pamela Kow";
            e2.Username = "employee2";
            e2.Password = Crypto.Sha256("employee2");
            e2.Dept = d1;
            e2.JobTitle = DeptRole.Employee;
            e2.Role = DeptRole.Contact;

            Employee e3 = new Employee();
            e3.Name = "Prof Ezra Pound";
            e3.Username = "depthead";
            e3.Password = Crypto.Sha256("depthead");
            e3.Dept = d1;
            e3.JobTitle = DeptRole.DeptHead;
            e3.Role = DeptRole.DeptHead;

            Employee e4 = new Employee();
            e4.Name = "Mrs Jane Koh";
            e4.Username = "storeclerk";
            e4.Password = Crypto.Sha256("storeclerk");
            e4.Dept = d5;
            e4.JobTitle = DeptRole.StoreClerk;
            e4.Role = DeptRole.StoreClerk;

            Employee e5 = new Employee();
            e5.Name = "Mr Gary Lim";
            e5.Username = "supervisor";
            e5.Password = Crypto.Sha256("supervisor");
            e5.Dept = d5;
            e5.JobTitle = DeptRole.StoreSupervisor;
            e5.Role = DeptRole.StoreSupervisor;

            Employee e6 = new Employee();
            e6.Name = "Mrs Marilyn Monroe";
            e6.Username = "manager";
            e6.Password = Crypto.Sha256("manager");
            e6.Dept = d5;
            e6.JobTitle = DeptRole.StoreSupervisor;
            e6.Role = DeptRole.StoreSupervisor;

            Employee e7 = new Employee();
            e7.Name = "Mrs Marilyn ";
            e7.Username = "manager";
            e7.Password = Crypto.Sha256("manager");
            e7.Dept = d5;
            e7.JobTitle = DeptRole.DeptHead;
            e7.Role = DeptRole.DeptHead;

            Employee e8 = new Employee();
            e8.Name = "Mrs Lynh ";
            e8.Username = "manager";
            e8.Password = Crypto.Sha256("manager");
            e8.Dept = d5;
            e8.JobTitle = DeptRole.DeptRep;
            e8.Role = DeptRole.DeptRep;

            Employee e9 = new Employee();
            e9.Name = "Mrs Lin ";
            e9.Username = "manager";
            e9.Password = Crypto.Sha256("manager");
            e9.Dept = d2;
            e9.JobTitle = DeptRole.DeptHead;
            e9.Role = DeptRole.DeptHead;

            Employee e10 = new Employee();
            e10.Name = "Mr Ong ";
            e10.Username = "manager";
            e10.Password = Crypto.Sha256("manager");
            e10.Dept = d2;
            e10.JobTitle = DeptRole.DeptRep;
            e10.Role = DeptRole.DeptRep;


            Employee e11 = new Employee();
            e11.Name = "Mr Ken ";
            e11.Username = "manager";
            e11.Password = Crypto.Sha256("manager");
            e11.Dept = d3;
            e11.JobTitle = DeptRole.DeptRep;
            e11.Role = DeptRole.DeptRep;

            Employee e12 = new Employee();
            e12.Name = "Mr Ken ";
            e12.Username = "manager";
            e12.Password = Crypto.Sha256("manager");
            e12.Dept = d3;
            e12.JobTitle = DeptRole.DeptHead;
            e12.Role = DeptRole.DeptHead;



            Employee e13 = new Employee();
            e13.Name = "Mr Ken ";
            e13.Username = "manager";
            e13.Password = Crypto.Sha256("manager");
            e13.Dept = d4;
            e13.JobTitle = DeptRole.DeptHead;
            e13.Role = DeptRole.DeptHead;

            Employee e14 = new Employee();
            e14.Name = "Mr Ken ";
            e14.Username = "manager";
            e14.Password = Crypto.Sha256("manager");
            e14.Dept = d4;
            e14.JobTitle = DeptRole.DeptRep;
            e14.Role = DeptRole.DeptRep;

            Employee e15 = new Employee();
            e15.Name = "Mr Tom Hanks";
            e15.Username = "storesupervisor";
            e15.Password = Crypto.Sha256("storesupervisor");
            e15.Dept = d5;
            e15.JobTitle = DeptRole.StoreSupervisor;
            e15.Role = DeptRole.StoreSupervisor;

            Employee e16 = new Employee();
            e16.Name = "Mr Chris Hemsworth";
            e16.Username = "storemanager";
            e16.Password = Crypto.Sha256("storemanager");
            e16.Dept = d5;
            e16.JobTitle = DeptRole.StoreManager;
            e16.Role = DeptRole.StoreManager;

            dbContext.Add(e1);
            dbContext.Add(e2);
            dbContext.Add(e3);
            dbContext.Add(e4);
            dbContext.Add(e5);
            dbContext.Add(e6);
            dbContext.Add(e7);
            dbContext.Add(e8);
            dbContext.Add(e9);
            dbContext.Add(e10);
            dbContext.Add(e11);
            dbContext.Add(e12);
            dbContext.Add(e13);
            dbContext.Add(e14);
            dbContext.Add(e15);
            dbContext.Add(e16);


            // Adding Requisition

            //DeptRequisition dr1 = new DeptRequisition();
            //dr1.Employee = e1;
            //dr1.RequisitionApprovalStatus = RequisitionApprovalStatus.Pending;
            //dr1.SubmissionStatus = SubmissionStatus.Submitted;

            //DeptRequisition dr2 = new DeptRequisition();
            //dr2.Employee = e1;
            //dr2.RequisitionApprovalStatus = RequisitionApprovalStatus.Pending;
            //dr2.SubmissionStatus = SubmissionStatus.Submitted;

            //DeptRequisition dr3 = new DeptRequisition();
            //dr3.Employee = e1;
            //dr3.RequisitionApprovalStatus = RequisitionApprovalStatus.Pending;
            //dr3.SubmissionStatus = SubmissionStatus.Submitted;

            //DeptRequisition dr4 = new DeptRequisition();
            //dr4.Employee = e1;
            //dr4.RequisitionApprovalStatus = RequisitionApprovalStatus.Approved;
            //dr4.SubmissionStatus = SubmissionStatus.Submitted;
            //dr4.RequisitionFulfillmentStatus = RequisitionFulfillmentStatus.ToBeProcessed;


            //DeptRequisition dr5 = new DeptRequisition();
            //dr5.Employee = e1;
            //dr5.RequisitionApprovalStatus = RequisitionApprovalStatus.Approved;
            //dr5.SubmissionStatus = SubmissionStatus.Submitted;
            //dr5.RequisitionFulfillmentStatus = RequisitionFulfillmentStatus.ToBeProcessed;

            //DeptRequisition dr6 = new DeptRequisition();
            //dr6.Employee = e1;
            //dr6.RequisitionApprovalStatus = RequisitionApprovalStatus.Approved;
            //dr6.SubmissionStatus = SubmissionStatus.Submitted;
            //dr6.RequisitionFulfillmentStatus = RequisitionFulfillmentStatus.ToBeProcessed;

            //DeptRequisition dr7 = new DeptRequisition();
            //dr7.Employee = e1;
            //dr7.SubmissionStatus = SubmissionStatus.Submitted;
            //dr7.RequisitionApprovalStatus = RequisitionApprovalStatus.Rejected;

            //DeptRequisition dr8 = new DeptRequisition();
            //dr8.Employee = e1;
            //dr8.SubmissionStatus = SubmissionStatus.Submitted;
            //dr8.RequisitionApprovalStatus = RequisitionApprovalStatus.Rejected;

            //DeptRequisition dr9 = new DeptRequisition();
            //dr9.Employee = e1;
            //dr9.SubmissionStatus = SubmissionStatus.Submitted;
            //dr9.RequisitionApprovalStatus = RequisitionApprovalStatus.Rejected;

            //DeptRequisition dr10 = new DeptRequisition();
            //dr10.Employee = e1;
            //dr10.RequisitionApprovalStatus = RequisitionApprovalStatus.Rejected;

            //DeptRequisition dr11 = new DeptRequisition();
            //dr11.Employee = e1;
            //dr11.RequisitionApprovalStatus = RequisitionApprovalStatus.Approved;

            //dbContext.Add(dr1);
            //dbContext.Add(dr2);
            //dbContext.Add(dr3);
            //dbContext.Add(dr4);
            //dbContext.Add(dr5);
            //dbContext.Add(dr6);
            //dbContext.Add(dr7);
            //dbContext.Add(dr8);
            //dbContext.Add(dr9);
            //dbContext.Add(dr10);
            //dbContext.Add(dr11);

            // Adding Requisition Details

            //RequisitionDetail rd1 = new RequisitionDetail();
            //rd1.Stationery = s1;
            //rd1.Qty = 10;
            //rd1.DeptRequisition = dr1;

            //RequisitionDetail rd2 = new RequisitionDetail();
            //rd2.Stationery = s3;
            //rd2.Qty = 45;
            //rd2.DeptRequisition = dr1;

            //RequisitionDetail rd3 = new RequisitionDetail();
            //rd3.Stationery = s5;
            //rd3.Qty = 100;
            //rd3.DeptRequisition = dr1;

            //RequisitionDetail rd4 = new RequisitionDetail();
            //rd4.Stationery = s7;
            //rd4.Qty = 25;
            //rd4.DeptRequisition = dr1;

            //RequisitionDetail rd5 = new RequisitionDetail();
            //rd5.Stationery = s9;
            //rd5.Qty = 25;
            //rd5.DeptRequisition = dr1;

            //dbContext.Add(rd1);
            //dbContext.Add(rd2);
            //dbContext.Add(rd3);
            //dbContext.Add(rd4);
            //dbContext.Add(rd5);

            //RequisitionDetail rd6 = new RequisitionDetail();
            //rd6.Stationery = s1;
            //rd6.Qty = 10;
            //rd6.DeptRequisition = dr2;

            //RequisitionDetail rd7 = new RequisitionDetail();
            //rd7.Stationery = s3;
            //rd7.Qty = 45;
            //rd7.DeptRequisition = dr3;

            //RequisitionDetail rd8 = new RequisitionDetail();
            //rd8.Stationery = s5;
            //rd8.Qty = 100;
            //rd8.DeptRequisition = dr4;

            //RequisitionDetail rd9 = new RequisitionDetail();
            //rd9.Stationery = s7;
            //rd9.Qty = 25;
            //rd9.DeptRequisition = dr5;

            //RequisitionDetail rd10 = new RequisitionDetail();
            //rd10.Stationery = s9;
            //rd10.Qty = 25;
            //rd10.DeptRequisition = dr6;

            //RequisitionDetail rd11 = new RequisitionDetail();
            //rd11.Stationery = s9;
            //rd11.Qty = 25;
            //rd11.DeptRequisition = dr7;

            //RequisitionDetail rd12 = new RequisitionDetail();
            //rd12.Stationery = s9;
            //rd12.Qty = 25;
            //rd12.DeptRequisition = dr8;

            //RequisitionDetail rd13 = new RequisitionDetail();
            //rd13.Stationery = s9;
            //rd13.Qty = 25;
            //rd13.DeptRequisition = dr9;

            //dbContext.Add(rd6);
            //dbContext.Add(rd7);
            //dbContext.Add(rd8);
            //dbContext.Add(rd9);
            //dbContext.Add(rd10);
            //dbContext.Add(rd11);
            //dbContext.Add(rd12);
            //dbContext.Add(rd13);

            //RequisitionDetail rd14 = new RequisitionDetail();
            //rd14.Stationery = s1;
            //rd14.Qty = 25;
            //rd14.DeptRequisition = dr11;

            //RequisitionDetail rd15 = new RequisitionDetail();
            //rd15.Stationery = s2;
            //rd15.Qty = 25;
            //rd15.DeptRequisition = dr11;

            //RequisitionDetail rd16 = new RequisitionDetail();
            //rd16.Stationery = s3;
            //rd16.Qty = 25;
            //rd16.DeptRequisition = dr11;

            //RequisitionDetail rd17 = new RequisitionDetail();
            //rd17.Stationery = s4;
            //rd17.Qty = 25;
            //rd17.DeptRequisition = dr11;

            //RequisitionDetail rd18 = new RequisitionDetail();
            //rd18.Stationery = s5;
            //rd18.Qty = 25;
            //rd18.DeptRequisition = dr11;

            //RequisitionDetail rd19 = new RequisitionDetail();
            //rd19.Stationery = s6;
            //rd19.Qty = 25;
            //rd19.DeptRequisition = dr11;

            //RequisitionDetail rd20 = new RequisitionDetail();
            //rd20.Stationery = s7;
            //rd20.Qty = 25;
            //rd20.DeptRequisition = dr11;

            //RequisitionDetail rd21 = new RequisitionDetail();
            //rd21.Stationery = s8;
            //rd21.Qty = 25;
            //rd21.DeptRequisition = dr11;

            //RequisitionDetail rd22 = new RequisitionDetail();
            //rd22.Stationery = s9;
            //rd22.Qty = 25;
            //rd22.DeptRequisition = dr11;

            //RequisitionDetail rd23 = new RequisitionDetail();
            //rd23.Stationery = s10;
            //rd23.Qty = 25;
            //rd23.DeptRequisition = dr11;

            //RequisitionDetail rd24 = new RequisitionDetail();
            //rd24.Stationery = s11;
            //rd24.Qty = 25;
            //rd24.DeptRequisition = dr11;

            //RequisitionDetail rd25 = new RequisitionDetail();
            //rd25.Stationery = s12;
            //rd25.Qty = 25;
            //rd25.DeptRequisition = dr11;

            //RequisitionDetail rd26 = new RequisitionDetail();
            //rd26.Stationery = s13;
            //rd26.Qty = 25;
            //rd26.DeptRequisition = dr11;

            //RequisitionDetail rd27 = new RequisitionDetail();
            //rd27.Stationery = s14;
            //rd27.Qty = 25;
            //rd27.DeptRequisition = dr11;

            //RequisitionDetail rd28 = new RequisitionDetail();
            //rd28.Stationery = s15;
            //rd28.Qty = 25;
            //rd28.DeptRequisition = dr11;

            //RequisitionDetail rd29 = new RequisitionDetail();
            //rd29.Stationery = s16;
            //rd29.Qty = 25;
            //rd29.DeptRequisition = dr11;

            //RequisitionDetail rd30 = new RequisitionDetail();
            //rd30.Stationery = s17;
            //rd30.Qty = 25;
            //rd30.DeptRequisition = dr11;

            //RequisitionDetail rd31 = new RequisitionDetail();
            //rd31.Stationery = s18;
            //rd31.Qty = 25;
            //rd31.DeptRequisition = dr11;

            //RequisitionDetail rd32 = new RequisitionDetail();
            //rd32.Stationery = s19;
            //rd32.Qty = 25;
            //rd32.DeptRequisition = dr11;

            //RequisitionDetail rd33 = new RequisitionDetail();
            //rd33.Stationery = s20;
            //rd33.Qty = 25;
            //rd33.DeptRequisition = dr11;

            //RequisitionDetail rd34 = new RequisitionDetail();
            //rd34.Stationery = s21;
            //rd34.Qty = 25;
            //rd34.DeptRequisition = dr11;

            //RequisitionDetail rd35 = new RequisitionDetail();
            //rd35.Stationery = s22;
            //rd35.Qty = 25;
            //rd35.DeptRequisition = dr11;

            //RequisitionDetail rd36 = new RequisitionDetail();
            //rd36.Stationery = s23;
            //rd36.Qty = 25;
            //rd36.DeptRequisition = dr11;

            //RequisitionDetail rd37 = new RequisitionDetail();
            //rd37.Stationery = s24;
            //rd37.Qty = 25;
            //rd37.DeptRequisition = dr11;

            //dbContext.Add(rd14);
            //dbContext.Add(rd15);
            //dbContext.Add(rd16);
            //dbContext.Add(rd17);
            //dbContext.Add(rd18);
            //dbContext.Add(rd19);
            //dbContext.Add(rd20);
            //dbContext.Add(rd21);
            //dbContext.Add(rd22);
            //dbContext.Add(rd23);
            //dbContext.Add(rd24);
            //dbContext.Add(rd25);
            //dbContext.Add(rd26);
            //dbContext.Add(rd27);
            //dbContext.Add(rd28);
            //dbContext.Add(rd29);
            //dbContext.Add(rd30);
            //dbContext.Add(rd31);
            //dbContext.Add(rd32);
            //dbContext.Add(rd33);
            //dbContext.Add(rd34);
            //dbContext.Add(rd35);
            //dbContext.Add(rd36);
            //dbContext.Add(rd37);



            // Add Supplier
            Supplier su1 = new Supplier();
            su1.Name = "FairPrice";
            su1.TelephoneNo = "8668 8668";
            su1.Address = "Ang Mo Kio";
            su1.supplierStatus = SupplierStatus.ContractApproved;

            Supplier su2 = new Supplier();
            su2.Name = "ColdStorage";
            su2.TelephoneNo = "8224 4228";
            su2.Address = "Kovan";
            su2.supplierStatus = SupplierStatus.ContractApproved;

            Supplier su3 = new Supplier();
            su3.Name = "Metro";
            su3.TelephoneNo = "8112 2118";
            su3.Address = "Marina Bay";
            su3.supplierStatus = SupplierStatus.ContractApproved;

            Supplier su4 = new Supplier();
            su4.Name = "Giant";
            su4.TelephoneNo = "8334 4338";
            su4.Address = "Bukit Batok";
            su4.supplierStatus = SupplierStatus.ContractApproved;

            Supplier su5 = new Supplier();
            su5.Name = "Citimart";
            su5.TelephoneNo = "86664 4668";
            su5.Address = "Jurong East";
            su5.supplierStatus = SupplierStatus.ContractApproved;

            dbContext.Add(su1);
            dbContext.Add(su2);
            dbContext.Add(su3);
            dbContext.Add(su4);
            dbContext.Add(su5);

            //Add SupplierDetail
            SupplierDetail suD1 = new SupplierDetail();
            suD1.Stationery = s1;
            suD1.Supplier = su1;
            suD1.UnitPrice = 10;

            SupplierDetail suD2 = new SupplierDetail();
            suD2.Stationery = s2;
            suD2.Supplier = su1;
            suD2.UnitPrice = 15;

            SupplierDetail suD3 = new SupplierDetail();
            suD3.Stationery = s3;
            suD3.Supplier = su2;
            suD3.UnitPrice = 18;

            SupplierDetail suD4 = new SupplierDetail();
            suD4.Stationery = s4;
            suD4.Supplier = su2;
            suD4.UnitPrice = 10;

            SupplierDetail suD5 = new SupplierDetail();
            suD5.Stationery = s5;
            suD5.Supplier = su3;
            suD5.UnitPrice = 12;

            SupplierDetail suD6 = new SupplierDetail();
            suD6.Stationery = s6;
            suD6.Supplier = su4;
            suD6.UnitPrice = 10;

            SupplierDetail suD7 = new SupplierDetail();
            suD7.Stationery = s7;
            suD7.Supplier = su4;
            suD7.UnitPrice = 15;

            SupplierDetail suD8 = new SupplierDetail();
            suD8.Stationery = s8;
            suD8.Supplier = su5;
            suD8.UnitPrice = 10;

            SupplierDetail suD9 = new SupplierDetail();
            suD9.Stationery = s9;
            suD9.Supplier = su5;
            suD9.UnitPrice = 20;

            dbContext.Add(suD1);
            dbContext.Add(suD2);
            dbContext.Add(suD3);
            dbContext.Add(suD4);
            dbContext.Add(suD5);
            dbContext.Add(suD6);
            dbContext.Add(suD7);
            dbContext.Add(suD8);
            dbContext.Add(suD9);

            //Add PO
            PO po1 = new PO();
            po1.OrderDate = new DateTime(2020, 10, 10);
            po1.POStatus = POStatus.Submitted;
            po1.POStatus = POStatus.Processing;
            po1.Supplier = su1;

            PO po2 = new PO();
            po2.OrderDate = new DateTime(2020, 8, 20);
            po2.POStatus = POStatus.Submitted;
            po2.POStatus = POStatus.Processing;
            po2.Supplier = su2;

            dbContext.Add(po1);


            //Add PODetail
            PODetail poD1 = new PODetail();
            poD1.Qty = 10;
            poD1.SupplierDetail = suD1;
            poD1.PO = po1;
            poD1.SupplierDetail.UnitPrice = 10;

            PODetail poD2 = new PODetail();
            poD2.Qty = 20;
            poD2.SupplierDetail = suD2;
            poD2.PO = po1;
            poD2.SupplierDetail.UnitPrice = 10;

            PODetail poD3 = new PODetail();
            poD3.Qty = 10;
            poD3.SupplierDetail = suD3;
            poD3.PO = po2;
            poD3.SupplierDetail.UnitPrice = 10;

            PODetail poD4 = new PODetail();
            poD4.Qty = 30;
            poD4.SupplierDetail = suD4;
            poD4.PO = po2;
            poD4.SupplierDetail.UnitPrice = 10;

            dbContext.Add(poD1);
            dbContext.Add(poD2);
            dbContext.Add(poD3);
            dbContext.Add(poD4);

            //Disbursement disbursement1 = new Disbursement();
            //disbursement1.DeptRequisition = dr1;
            //Disbursement disbursement2 = new Disbursement();
            //disbursement2.DeptRequisition = dr2;
            //Disbursement disbursement3 = new Disbursement();
            //disbursement3.DeptRequisition = dr3;

            //DisbursementDetail dd1 = new DisbursementDetail();
            //dd1.Disbursement = disbursement1;
            //dd1.Stationery = s1;

            //DisbursementDetail dd2 = new DisbursementDetail();
            //dd2.Disbursement = disbursement2;
            //dd2.Stationery = s2;

            //DisbursementDetail dd3 = new DisbursementDetail();
            //dd3.Disbursement = disbursement3;
            //dd3.Stationery = s3;

            //dbContext.Add(disbursement1);
            //dbContext.Add(disbursement2);
            //dbContext.Add(disbursement3);
            //dbContext.Add(dd1);
            //dbContext.Add(dd2);
            //dbContext.Add(dd3);

            //UsageHistory usage11m1 = new UsageHistory();
            //usage11m1.Stationery = s1;
            //usage11m1.Department = d1;
            //usage11m1.Qty = 15;
            //usage11m1.A_Date = new DateTime(2020, 01, 25);
            //usage11m1.DisbursementDetail = dd1;

            //UsageHistory usage11m2 = new UsageHistory();
            //usage11m2.Stationery = s1;
            //usage11m2.Department = d1;
            //usage11m2.Qty = 25;
            //usage11m2.A_Date = new DateTime(2020, 02, 25);

            //UsageHistory usage11m3 = new UsageHistory();
            //usage11m3.Stationery = s1;
            //usage11m3.Department = d1;
            //usage11m3.Qty = 80;
            //usage11m3.A_Date = new DateTime(2020, 03, 25);



            //UsageHistory usage12m1 = new UsageHistory();
            //usage12m1.Stationery = s2;
            //usage12m1.Department = d1;
            //usage12m1.Qty = 15;
            //usage12m1.A_Date = new DateTime(2020, 01, 25);
            //usage12m1.DisbursementDetail = dd1;

            //UsageHistory usage12m2 = new UsageHistory();
            //usage12m2.Stationery = s2;
            //usage12m2.Department = d1;
            //usage12m2.Qty = 25;
            //usage12m2.A_Date = new DateTime(2020, 02, 25);

            //UsageHistory usage12m3 = new UsageHistory();
            //usage12m3.Stationery = s2;
            //usage12m3.Department = d1;
            //usage12m3.Qty = 80;
            //usage12m3.A_Date = new DateTime(2020, 03, 25);


            //UsageHistory usage13m1 = new UsageHistory();
            //usage13m1.Stationery = s3;
            //usage13m1.Department = d1;
            //usage13m1.Qty = 20;
            //usage13m1.A_Date = new DateTime(2020, 01, 25);
            //usage13m1.DisbursementDetail = dd1;

            //UsageHistory usage13m2 = new UsageHistory();
            //usage13m2.Stationery = s3;
            //usage13m2.Department = d1;
            //usage13m2.Qty = 30;
            //usage13m2.A_Date = new DateTime(2020, 02, 25);

            //UsageHistory usage13m3 = new UsageHistory();
            //usage13m3.Stationery = s3;
            //usage13m3.Department = d1;
            //usage13m3.Qty = 85;
            //usage13m3.A_Date = new DateTime(2020, 03, 25);


            //UsageHistory usage21m1 = new UsageHistory();
            //usage21m1.Stationery = s1;
            //usage21m1.Department = d2;
            //usage21m1.Qty = 150;
            //usage21m1.A_Date = new DateTime(2020, 01, 25);

            //UsageHistory usage21m2 = new UsageHistory();
            //usage21m2.Stationery = s1;
            //usage21m2.Department = d2;
            //usage21m2.Qty = 250;
            //usage21m2.A_Date = new DateTime(2020, 02, 25);

            //UsageHistory usage21m3 = new UsageHistory();
            //usage21m3.Stationery = s1;
            //usage21m3.Department = d2;
            //usage21m3.Qty = 80;
            //usage21m3.A_Date = new DateTime(2020, 03, 25);


            //UsageHistory usage22m1 = new UsageHistory();
            //usage22m1.Stationery = s2;
            //usage22m1.Department = d2;
            //usage22m1.Qty = 220;
            //usage22m1.A_Date = new DateTime(2020, 01, 25);

            //UsageHistory usage22m2 = new UsageHistory();
            //usage22m2.Stationery = s2;
            //usage22m2.Department = d2;
            //usage22m2.Qty = 230;
            //usage22m2.A_Date = new DateTime(2020, 02, 25);

            //UsageHistory usage22m3 = new UsageHistory();
            //usage22m3.Stationery = s2;
            //usage22m3.Department = d2;
            //usage22m3.Qty = 285;
            //usage22m3.A_Date = new DateTime(2020, 03, 25);


            //UsageHistory usage23m1 = new UsageHistory();
            //usage23m1.Stationery = s3;
            //usage23m1.Department = d2;
            //usage23m1.Qty = 320;
            //usage23m1.A_Date = new DateTime(2020, 01, 25);

            //UsageHistory usage23m2 = new UsageHistory();
            //usage23m2.Stationery = s3;
            //usage23m2.Department = d2;
            //usage23m2.Qty = 330;
            //usage23m2.A_Date = new DateTime(2020, 02, 25);

            //UsageHistory usage23m3 = new UsageHistory();
            //usage23m3.Stationery = s3;
            //usage23m3.Department = d2;
            //usage23m3.Qty = 285;
            //usage23m3.A_Date = new DateTime(2020, 03, 25);




            //UsageHistory usage31m1 = new UsageHistory();
            //usage31m1.Stationery = s1;
            //usage31m1.Department = d3;
            //usage31m1.Qty = 40;
            //usage31m1.A_Date = new DateTime(2020, 01, 25);

            //UsageHistory usage31m2 = new UsageHistory();
            //usage31m2.Stationery = s1;
            //usage31m2.Department = d3;
            //usage31m2.Qty = 150;
            //usage31m2.A_Date = new DateTime(2020, 02, 25);

            //UsageHistory usage31m3 = new UsageHistory();
            //usage31m3.Stationery = s1;
            //usage31m3.Department = d3;
            //usage31m3.Qty = 300;
            //usage31m3.A_Date = new DateTime(2020, 03, 25);

            //UsageHistory usage32m1 = new UsageHistory();
            //usage32m1.Stationery = s2;
            //usage32m1.Department = d3;
            //usage32m1.Qty = 80;

            //usage32m1.A_Date = new DateTime(2020, 01, 25);

            //UsageHistory usage32m2 = new UsageHistory();
            //usage32m2.Stationery = s2;
            //usage32m2.Department = d3;
            //usage32m2.Qty = 200;
            //usage32m2.A_Date = new DateTime(2020, 02, 25);

            //UsageHistory usage32m3 = new UsageHistory();
            //usage32m3.Stationery = s2;
            //usage32m3.Department = d3;
            //usage32m3.Qty = 400;
            //usage32m3.A_Date = new DateTime(2020, 03, 25);

            //UsageHistory usage33m1 = new UsageHistory();
            //usage33m1.Stationery = s3;
            //usage33m1.Department = d3;
            //usage33m1.Qty = 280;

            //usage33m1.A_Date = new DateTime(2020, 01, 25);

            //UsageHistory usage33m2 = new UsageHistory();
            //usage33m2.Stationery = s3;
            //usage33m2.Department = d3;
            //usage33m2.Qty = 200;
            //usage33m2.A_Date = new DateTime(2020, 02, 25);

            //UsageHistory usage33m3 = new UsageHistory();
            //usage33m3.Stationery = s3;
            //usage33m3.Department = d3;
            //usage33m3.Qty = 400;
            //usage33m3.A_Date = new DateTime(2020, 03, 25);

            //dbContext.Add(usage11m1);
            //dbContext.Add(usage12m1);
            //dbContext.Add(usage13m1);
            //dbContext.Add(usage21m1);
            //dbContext.Add(usage22m1);
            //dbContext.Add(usage23m1);
            //dbContext.Add(usage31m1);
            //dbContext.Add(usage32m1);
            //dbContext.Add(usage33m1);

            //dbContext.Add(usage11m2);
            //dbContext.Add(usage12m2);
            //dbContext.Add(usage13m2);
            //dbContext.Add(usage21m2);
            //dbContext.Add(usage22m2);
            //dbContext.Add(usage23m2);
            //dbContext.Add(usage31m2);
            //dbContext.Add(usage32m2);
            //dbContext.Add(usage33m2);

            //dbContext.Add(usage11m3);
            //dbContext.Add(usage12m3);
            //dbContext.Add(usage13m3);
            //dbContext.Add(usage21m3);
            //dbContext.Add(usage22m3);
            //dbContext.Add(usage23m3);
            //dbContext.Add(usage31m3);
            //dbContext.Add(usage32m3);
            //dbContext.Add(usage33m3);


            //Ben
            //create disbursementid
            //Disbursement disbursement1 = new Disbursement();
            //disbursement1.DeptRequisition = dr1;
            //disbursement1.DeptRequisition.Employee.Dept = d1;

            //Disbursement disbursement2 = new Disbursement();
            //disbursement2.DeptRequisition = dr2;
            //disbursement2.DeptRequisition.Employee.Dept = d1;

            //Disbursement disbursement3 = new Disbursement();
            //disbursement3.DeptRequisition = dr3;
            //disbursement3.DeptRequisition.Employee.Dept = d1;

            //Disbursement disbursement4 = new Disbursement();
            //disbursement4.DeptRequisition = dr4;
            //disbursement4.DeptRequisition.Employee.Dept = d1;

            //Disbursement disbursement5 = new Disbursement();
            //disbursement5.DeptRequisition = dr5;
            //disbursement5.DeptRequisition.Employee.Dept = d1;

            //Disbursement disbursement6 = new Disbursement();
            //disbursement6.DeptRequisition = dr6;
            //disbursement6.DeptRequisition.Employee.Dept = d1;

            //Disbursement disbursement7 = new Disbursement();
            //disbursement7.DeptRequisition = dr7;
            //disbursement7.DeptRequisition.Employee.Dept = d1;

            //Disbursement disbursement8 = new Disbursement();
            //disbursement8.DeptRequisition = dr8;
            //disbursement8.DeptRequisition.Employee.Dept = d1;

            //Disbursement disbursement9 = new Disbursement();
            //disbursement9.DeptRequisition = dr9;
            //disbursement9.DeptRequisition.Employee.Dept = d1;

            //Disbursement disbursement10 = new Disbursement();
            //disbursement10.DeptRequisition = dr10;
            //disbursement10.DeptRequisition.Employee.Dept = d1;


            //DisbursementDetail dd1 = new DisbursementDetail();
            //dd1.Disbursement = disbursement1;
            //dd1.Stationery = s1;
            //DisbursementDetail dd2 = new DisbursementDetail();
            //dd2.Disbursement = disbursement2;
            //dd2.Stationery = s2;
            //DisbursementDetail dd3 = new DisbursementDetail();
            //dd3.Disbursement = disbursement3;
            //dd3.Stationery = s3;

            //dbContext.Add(disbursement1);
            //dbContext.Add(disbursement2);
            //dbContext.Add(disbursement3);
            //dbContext.Add(disbursement4);
            //dbContext.Add(disbursement5);
            //dbContext.Add(disbursement6);
            //dbContext.Add(disbursement7);
            //dbContext.Add(disbursement8);
            //dbContext.Add(disbursement9);
            //dbContext.Add(disbursement10);
            //dbContext.Add(dd1);
            //dbContext.Add(dd2);
            //dbContext.Add(dd3);


            //start by Saw

            Disbursement dbur1 = new Disbursement();
            Disbursement dbur2 = new Disbursement();
            Disbursement dbur3 = new Disbursement();
            Disbursement dbur4 = new Disbursement();
            Disbursement dbur5 = new Disbursement();
            Disbursement dbur6 = new Disbursement();
            Disbursement dbur7 = new Disbursement();
            Disbursement dbur8 = new Disbursement();
            Disbursement dbur9 = new Disbursement();
            Disbursement dbur10 = new Disbursement();
            Disbursement dbur11 = new Disbursement();
            Disbursement dbur12 = new Disbursement();
            Disbursement dbur13 = new Disbursement();
            Disbursement dbur14 = new Disbursement();

            dbContext.Add(dbur1);
            dbContext.Add(dbur2);
            dbContext.Add(dbur3);
            dbContext.Add(dbur4);
            dbContext.Add(dbur5);
            dbContext.Add(dbur6);
            dbContext.Add(dbur7);
            dbContext.Add(dbur8);
            dbContext.Add(dbur9);
            dbContext.Add(dbur10);
            dbContext.Add(dbur11);
            dbContext.Add(dbur12);
            dbContext.Add(dbur13);
            dbContext.Add(dbur14);

            DisbursementDetail dd1dbur1 = new DisbursementDetail();
            dd1dbur1.A_Date = new DateTime(2020, 1, 25);
            dd1dbur1.Department = d4;
            dd1dbur1.Disbursement = dbur1;
            dd1dbur1.Stationery = s13;
            dd1dbur1.Qty = 175;
            dd1dbur1.Month = 1;
            dd1dbur1.Year = 2020;
            dbContext.Add(dd1dbur1);

            DisbursementDetail dd2dbur1 = new DisbursementDetail();
            dd2dbur1.A_Date = new DateTime(2020, 1, 25);
            dd2dbur1.Department = d4;
            dd2dbur1.Disbursement = dbur1;
            dd2dbur1.Stationery = s14;
            dd2dbur1.Qty = 180;
            dd2dbur1.Month = 1;
            dd2dbur1.Year = 2020;
            dbContext.Add(dd2dbur1);

            DisbursementDetail dd3dbur1 = new DisbursementDetail();
            dd3dbur1.A_Date = new DateTime(2020, 1, 25);
            dd3dbur1.Department = d4;
            dd3dbur1.Disbursement = dbur1;
            dd3dbur1.Stationery = s15;
            dd3dbur1.Qty = 185;
            dd3dbur1.Month = 1;
            dd3dbur1.Year = 2020;
            dbContext.Add(dd3dbur1);

            DisbursementDetail dd1dbur2 = new DisbursementDetail();
            dd1dbur2.A_Date = new DateTime(2020, 2, 25);
            dd1dbur2.Department = d4;
            dd1dbur2.Disbursement = dbur2;
            dd1dbur2.Stationery = s13;
            dd1dbur2.Qty = 160;
            dd1dbur2.Month = 2;
            dd1dbur2.Year = 2020;
            dbContext.Add(dd1dbur2);

            DisbursementDetail dd2dbur2 = new DisbursementDetail();
            dd2dbur2.A_Date = new DateTime(2020, 2, 25);
            dd2dbur2.Department = d4;
            dd2dbur2.Disbursement = dbur2;
            dd2dbur2.Stationery = s14;
            dd2dbur2.Qty = 275;
            dd2dbur2.Month = 2;
            dd2dbur2.Year = 2020;
            dbContext.Add(dd2dbur2);

            DisbursementDetail dd3dbur2 = new DisbursementDetail();
            dd3dbur2.A_Date = new DateTime(2020, 2, 25);
            dd3dbur2.Department = d4;
            dd3dbur2.Disbursement = dbur2;
            dd3dbur2.Stationery = s15;
            dd3dbur2.Qty = 75;
            dd3dbur2.Month = 2;
            dd3dbur2.Year = 2020;
            dbContext.Add(dd3dbur2);


            DisbursementDetail dd1dbur3 = new DisbursementDetail();
            dd1dbur3.A_Date = new DateTime(2020, 3, 25);
            dd1dbur3.Department = d4;
            dd1dbur3.Disbursement = dbur3;
            dd1dbur3.Stationery = s13;
            dd1dbur3.Qty = 175;
            dd1dbur3.Month = 3;
            dd1dbur3.Year = 2020;
            dbContext.Add(dd1dbur3);

            DisbursementDetail dd2dbur3 = new DisbursementDetail();
            dd2dbur3.A_Date = new DateTime(2020, 3, 25);
            dd2dbur3.Department = d4;
            dd2dbur3.Disbursement = dbur3;
            dd2dbur3.Stationery = s14;
            dd2dbur3.Qty = 175;
            dd2dbur3.Month = 3;
            dd2dbur3.Year = 2020;
            dbContext.Add(dd2dbur3);

            DisbursementDetail dd3dbur3 = new DisbursementDetail();
            dd3dbur3.A_Date = new DateTime(2020, 3, 25);
            dd3dbur3.Department = d4;
            dd3dbur3.Disbursement = dbur3;
            dd3dbur3.Stationery = s15;
            dd3dbur3.Qty = 175;
            dd3dbur3.Month = 3;
            dd3dbur3.Year = 2020;
            dbContext.Add(dd3dbur3);


            DisbursementDetail dd1dbur4 = new DisbursementDetail();
            dd1dbur4.A_Date = new DateTime(2020, 4, 25);
            dd1dbur4.Department = d4;
            dd1dbur4.Disbursement = dbur4;
            dd1dbur4.Stationery = s13;
            dd1dbur4.Qty = 88;
            dd1dbur4.Month = 4;
            dd1dbur4.Year = 2020;
            dbContext.Add(dd1dbur4);

            DisbursementDetail dd2dbur4 = new DisbursementDetail();
            dd2dbur4.A_Date = new DateTime(2020, 4, 25);
            dd2dbur4.Department = d4;
            dd2dbur4.Disbursement = dbur4;
            dd2dbur4.Stationery = s14;
            dd2dbur4.Qty = 288;
            dd2dbur4.Month = 4;
            dd2dbur4.Year = 2020;
            dbContext.Add(dd2dbur4);

            DisbursementDetail dd3dbur4 = new DisbursementDetail();
            dd3dbur4.A_Date = new DateTime(2020, 4, 25);
            dd3dbur4.Department = d4;
            dd3dbur4.Disbursement = dbur4;
            dd3dbur4.Stationery = s15;
            dd3dbur4.Qty = 379;
            dd3dbur4.Month = 4;
            dd3dbur4.Year = 2020;
            dbContext.Add(dd3dbur4);


            DisbursementDetail dd1dbur5 = new DisbursementDetail();
            dd1dbur5.A_Date = new DateTime(2020, 5, 25);
            dd1dbur5.Department = d4;
            dd1dbur5.Disbursement = dbur5;
            dd1dbur5.Stationery = s13;
            dd1dbur5.Qty = 157;
            dd1dbur5.Month = 5;
            dd1dbur5.Year = 2020;
            dbContext.Add(dd1dbur5);

            DisbursementDetail dd2dbur5 = new DisbursementDetail();
            dd2dbur5.A_Date = new DateTime(2020, 5, 25);
            dd2dbur5.Department = d4;
            dd2dbur5.Disbursement = dbur5;
            dd2dbur5.Stationery = s14;
            dd2dbur5.Qty = 340;
            dd2dbur5.Month = 5;
            dd2dbur5.Year = 2020;
            dbContext.Add(dd2dbur5);

            DisbursementDetail dd3dbur5 = new DisbursementDetail();
            dd3dbur5.A_Date = new DateTime(2020, 5, 25);
            dd3dbur5.Department = d4;
            dd3dbur5.Disbursement = dbur5;
            dd3dbur5.Stationery = s15;
            dd3dbur5.Qty = 485;
            dd3dbur5.Month = 5;
            dd3dbur5.Year = 2020;
            dbContext.Add(dd3dbur5);


            DisbursementDetail dd1dbur6 = new DisbursementDetail();
            dd1dbur6.A_Date = new DateTime(2020, 6, 25);
            dd1dbur6.Department = d4;
            dd1dbur6.Disbursement = dbur6;
            dd1dbur6.Stationery = s13;
            dd1dbur6.Qty = 270;
            dd1dbur6.Month = 6;
            dd1dbur6.Year = 2020;
            dbContext.Add(dd1dbur6);

            DisbursementDetail dd2dbur6 = new DisbursementDetail();
            dd2dbur6.A_Date = new DateTime(2020, 6, 25);
            dd2dbur6.Department = d4;
            dd2dbur6.Disbursement = dbur6;
            dd2dbur6.Stationery = s14;
            dd2dbur6.Qty = 335;
            dd2dbur6.Month = 6;
            dd2dbur6.Year = 2020;
            dbContext.Add(dd2dbur6);

            DisbursementDetail dd3dbur6 = new DisbursementDetail();
            dd3dbur6.A_Date = new DateTime(2020, 6, 25);
            dd3dbur6.Department = d4;
            dd3dbur6.Disbursement = dbur6;
            dd3dbur6.Stationery = s15;
            dd3dbur6.Qty = 245;
            dd3dbur6.Month = 6;
            dd3dbur6.Year = 2020;
            dbContext.Add(dd3dbur6);


            DisbursementDetail dd1dbur7 = new DisbursementDetail();
            dd1dbur7.A_Date = new DateTime(2020, 7, 25);
            dd1dbur7.Department = d4;
            dd1dbur7.Disbursement = dbur7;
            dd1dbur7.Stationery = s13;
            dd1dbur7.Qty = 95;
            dd1dbur7.Month = 7;
            dd1dbur7.Year = 2020;
            dbContext.Add(dd1dbur7);

            DisbursementDetail dd2dbur7 = new DisbursementDetail();
            dd2dbur7.A_Date = new DateTime(2020, 7, 25);
            dd2dbur7.Department = d4;
            dd2dbur7.Disbursement = dbur7;
            dd2dbur7.Stationery = s14;
            dd2dbur7.Qty = 135;
            dd2dbur7.Month = 7;
            dd2dbur7.Year = 2020;
            dbContext.Add(dd2dbur7);

            DisbursementDetail dd3dbur7 = new DisbursementDetail();
            dd3dbur7.A_Date = new DateTime(2020, 7, 25);
            dd3dbur7.Department = d4;
            dd3dbur7.Disbursement = dbur7;
            dd3dbur7.Stationery = s15;
            dd3dbur7.Qty = 226;
            dd3dbur7.Month = 7;
            dd3dbur7.Year = 2020;
            dbContext.Add(dd3dbur7);


            DisbursementDetail dd1dbur8 = new DisbursementDetail();
            dd1dbur8.A_Date = new DateTime(2020, 3, 25);
            dd1dbur8.Department = d5;
            dd1dbur8.Disbursement = dbur8;
            dd1dbur8.Stationery = s13;
            dd1dbur8.Qty = 90;
            dd1dbur8.Month = 3;
            dd1dbur8.Year = 2020;
            dbContext.Add(dd1dbur8);

            DisbursementDetail dd2dbur8 = new DisbursementDetail();
            dd2dbur8.A_Date = new DateTime(2020, 3, 25);
            dd2dbur8.Department = d5;
            dd2dbur8.Disbursement = dbur8;
            dd2dbur8.Stationery = s14;
            dd2dbur8.Qty = 180;
            dd2dbur8.Month = 3;
            dd2dbur8.Year = 2020;
            dbContext.Add(dd2dbur8);

            DisbursementDetail dd3dbur8 = new DisbursementDetail();
            dd3dbur8.A_Date = new DateTime(2020, 3, 25);
            dd3dbur8.Department = d5;
            dd3dbur8.Disbursement = dbur8;
            dd3dbur8.Stationery = s15;
            dd3dbur8.Qty = 400;
            dd3dbur8.Month = 3;
            dd3dbur8.Year = 2020;
            dbContext.Add(dd3dbur8);


            DisbursementDetail dd1dbur9 = new DisbursementDetail();
            dd1dbur9.A_Date = new DateTime(2020, 1, 25);
            dd1dbur9.Department = d5;
            dd1dbur9.Disbursement = dbur9;
            dd1dbur9.Stationery = s13;
            dd1dbur9.Qty = 100;
            dd1dbur9.Month = 1;
            dd1dbur9.Year = 2020;
            dbContext.Add(dd1dbur4);

            DisbursementDetail dd2dbur9 = new DisbursementDetail();
            dd2dbur9.A_Date = new DateTime(2020, 1, 25);
            dd2dbur9.Department = d5;
            dd2dbur9.Disbursement = dbur9;
            dd2dbur9.Stationery = s14;
            dd2dbur9.Qty = 250;
            dd2dbur9.Month = 1;
            dd2dbur9.Year = 2020;
            dbContext.Add(dd2dbur4);

            DisbursementDetail dd3dbur9 = new DisbursementDetail();
            dd3dbur4.A_Date = new DateTime(2020, 1, 25);
            dd3dbur4.Department = d5;
            dd3dbur4.Disbursement = dbur9;
            dd3dbur4.Stationery = s15;
            dd3dbur4.Qty = 345;
            dd3dbur4.Month = 1;
            dd3dbur4.Year = 2020;
            dbContext.Add(dd3dbur4);


            DisbursementDetail dd1dbur10 = new DisbursementDetail();
            dd1dbur10.A_Date = new DateTime(2020, 2, 25);
            dd1dbur10.Department = d5;
            dd1dbur10.Disbursement = dbur10;
            dd1dbur10.Stationery = s13;
            dd1dbur10.Qty = 95;
            dd1dbur10.Month = 2;
            dd1dbur10.Year = 2020;
            dbContext.Add(dd1dbur10);

            DisbursementDetail dd2dbur10 = new DisbursementDetail();
            dd2dbur10.A_Date = new DateTime(2020, 2, 25);
            dd2dbur10.Department = d5;
            dd2dbur10.Disbursement = dbur10;
            dd2dbur10.Stationery = s14;
            dd2dbur10.Qty = 278;
            dd2dbur10.Month = 2;
            dd2dbur10.Year = 2020;
            dbContext.Add(dd2dbur10);

            DisbursementDetail dd3dbur10 = new DisbursementDetail();
            dd3dbur10.A_Date = new DateTime(2020, 2, 25);
            dd3dbur10.Department = d5;
            dd3dbur10.Disbursement = dbur10;
            dd3dbur10.Stationery = s15;
            dd3dbur10.Qty = 360;
            dd3dbur10.Month = 2;
            dd3dbur10.Year = 2020;
            dbContext.Add(dd3dbur10);



            DisbursementDetail dd1dbur11 = new DisbursementDetail();
            dd1dbur11.A_Date = new DateTime(2020, 4, 25);
            dd1dbur11.Department = d5;
            dd1dbur11.Disbursement = dbur11;
            dd1dbur11.Stationery = s13;
            dd1dbur11.Qty = 120;
            dd1dbur11.Month = 4;
            dd1dbur11.Year = 2020;
            dbContext.Add(dd1dbur11);

            DisbursementDetail dd2dbur11 = new DisbursementDetail();
            dd2dbur11.A_Date = new DateTime(2020, 4, 25);
            dd2dbur11.Department = d5;
            dd2dbur11.Disbursement = dbur11;
            dd2dbur11.Stationery = s14;
            dd2dbur11.Qty = 245;
            dd2dbur11.Month = 4;
            dd2dbur11.Year = 2020;
            dbContext.Add(dd2dbur11);

            DisbursementDetail dd3dbur11 = new DisbursementDetail();
            dd3dbur11.A_Date = new DateTime(2020, 4, 25);
            dd3dbur11.Department = d5;
            dd3dbur11.Disbursement = dbur11;
            dd3dbur11.Stationery = s15;
            dd3dbur11.Qty = 315;
            dd3dbur11.Month = 4;
            dd3dbur11.Year = 2020;
            dbContext.Add(dd3dbur11);


            DisbursementDetail dd1dbur12 = new DisbursementDetail();
            dd1dbur12.A_Date = new DateTime(2020, 5, 25);
            dd1dbur12.Department = d5;
            dd1dbur12.Disbursement = dbur12;
            dd1dbur12.Stationery = s13;
            dd1dbur12.Qty = 110;
            dd1dbur12.Month = 5;
            dd1dbur12.Year = 2020;
            dbContext.Add(dd1dbur12);

            DisbursementDetail dd2dbur12 = new DisbursementDetail();
            dd2dbur12.A_Date = new DateTime(2020, 5, 25);
            dd2dbur12.Department = d5;
            dd2dbur12.Disbursement = dbur12;
            dd2dbur12.Stationery = s14;
            dd2dbur12.Qty = 230;
            dd2dbur12.Month = 5;
            dd2dbur12.Year = 2020;
            dbContext.Add(dd2dbur12);

            DisbursementDetail dd3dbur12 = new DisbursementDetail();
            dd3dbur12.A_Date = new DateTime(2020, 5, 25);
            dd3dbur12.Department = d5;
            dd3dbur12.Disbursement = dbur12;
            dd3dbur12.Stationery = s15;
            dd3dbur12.Qty = 345;
            dd3dbur12.Month = 5;
            dd3dbur12.Year = 2020;
            dbContext.Add(dd3dbur12);


            DisbursementDetail dd1dbur13 = new DisbursementDetail();
            dd1dbur13.A_Date = new DateTime(2020, 6, 25);
            dd1dbur13.Department = d5;
            dd1dbur13.Disbursement = dbur13;
            dd1dbur13.Stationery = s13;
            dd1dbur13.Qty = 130;
            dd1dbur13.Month = 6;
            dd1dbur13.Year = 2020;
            dbContext.Add(dd1dbur13);

            DisbursementDetail dd2dbur13 = new DisbursementDetail();
            dd2dbur13.A_Date = new DateTime(2020, 6, 25);
            dd2dbur13.Department = d5;
            dd2dbur13.Disbursement = dbur13;
            dd2dbur13.Stationery = s14;
            dd2dbur13.Qty = 225;
            dd2dbur13.Month = 6;
            dd2dbur13.Year = 2020;
            dbContext.Add(dd2dbur13);

            DisbursementDetail dd3dbur13 = new DisbursementDetail();
            dd3dbur13.A_Date = new DateTime(2020, 6, 25);
            dd3dbur13.Department = d5;
            dd3dbur13.Disbursement = dbur13;
            dd3dbur13.Stationery = s15;
            dd3dbur13.Qty = 380;
            dd3dbur13.Month = 6;
            dd3dbur13.Year = 2020;
            dbContext.Add(dd3dbur13);


            DisbursementDetail dd1dbur14 = new DisbursementDetail();
            dd1dbur14.A_Date = new DateTime(2020, 7, 25);
            dd1dbur14.Department = d5;
            dd1dbur14.Disbursement = dbur14;
            dd1dbur14.Stationery = s13;
            dd1dbur14.Qty = 125;
            dd1dbur14.Month = 7;
            dd1dbur14.Year = 2020;
            dbContext.Add(dd1dbur14);

            DisbursementDetail dd2dbur14 = new DisbursementDetail();
            dd2dbur14.A_Date = new DateTime(2020, 7, 25);
            dd2dbur14.Department = d5;
            dd2dbur14.Disbursement = dbur14;
            dd2dbur14.Stationery = s14;
            dd2dbur14.Qty = 250;
            dd2dbur14.Month = 7;
            dd2dbur14.Year = 2020;
            dbContext.Add(dd2dbur14);

            DisbursementDetail dd3dbur14 = new DisbursementDetail();
            dd3dbur14.A_Date = new DateTime(2020, 7, 25);
            dd3dbur14.Department = d5;
            dd3dbur14.Disbursement = dbur14;
            dd3dbur14.Stationery = s15;
            dd3dbur14.Qty = 320;
            dd3dbur14.Month = 7;
            dd3dbur14.Year = 2020;
            dbContext.Add(dd3dbur14);



            //Saw/Shermaine


            //Departmemt 3 , 1 Disbursement, 3 stationery, Jan
            DisbursementDetail product1m1d1 = new DisbursementDetail();
            product1m1d1.Stationery = s13;
            product1m1d1.Department = d3;
            product1m1d1.Qty = 200;
            product1m1d1.A_Date = new DateTime(2020, 01, 25);

            product1m1d1.Disbursement = dbur1;
            product1m1d1.Month = 1;
            product1m1d1.Year = 2020;

            DisbursementDetail product2m1d1 = new DisbursementDetail();
            product2m1d1.Stationery = s14;
            product2m1d1.Department = d3;
            product2m1d1.Qty = 100;
            product2m1d1.A_Date = new DateTime(2020, 01, 25);

            product2m1d1.Disbursement = dbur1;
            product2m1d1.Month = 1;
            product2m1d1.Year = 2020;

            DisbursementDetail product3m1d1 = new DisbursementDetail();
            product3m1d1.Stationery = s15;
            product3m1d1.Department = d3;
            product3m1d1.Qty = 200;
            product3m1d1.A_Date = new DateTime(2020, 01, 25);

            product3m1d1.Disbursement = dbur1;
            product3m1d1.Month = 1;
            product3m1d1.Year = 2020;

            dbContext.Add(product1m1d1);
            dbContext.Add(product2m1d1);
            dbContext.Add(product3m1d1);


            //Departmemt 3 , 1 Disbursement (2nd), 3 stationery, Feb

            DisbursementDetail product1m2d1 = new DisbursementDetail();
            product1m2d1.Stationery = s13;
            product1m2d1.Department = d3;
            product1m2d1.Qty = 150;
            product1m2d1.A_Date = new DateTime(2020, 02, 25);

            product1m2d1.Disbursement = dbur2;
            product1m2d1.Month = 2;
            product1m2d1.Year = 2020;

            DisbursementDetail product2m2d1 = new DisbursementDetail();
            product2m2d1.Stationery = s14;
            product2m2d1.Department = d3;
            product2m2d1.Qty = 100;
            product2m2d1.A_Date = new DateTime(2020, 02, 25);

            product2m2d1.Disbursement = dbur2;
            product2m2d1.Month = 2;
            product2m2d1.Year = 2020;

            DisbursementDetail product3m2d1 = new DisbursementDetail();
            product3m2d1.Stationery = s15;
            product3m2d1.Department = d3;
            product3m2d1.Qty = 80;
            product3m2d1.A_Date = new DateTime(2020, 02, 25);

            product3m2d1.Disbursement = dbur2;
            product3m2d1.Month = 2;
            product3m2d1.Year = 2020;

            dbContext.Add(product1m2d1);
            dbContext.Add(product2m2d1);
            dbContext.Add(product3m2d1);


            //Departmemt 3 , 1 Disbursement, 3 stationery, March

            DisbursementDetail product1m3d1 = new DisbursementDetail();
            product1m3d1.Stationery = s13;
            product1m3d1.Department = d3;
            product1m3d1.Qty = 300;
            product1m3d1.A_Date = new DateTime(2020, 03, 25);

            product1m3d1.Disbursement = dbur3;
            product1m3d1.Month = 3;
            product1m3d1.Year = 2020;

            DisbursementDetail product2m3d1 = new DisbursementDetail();
            product2m3d1.Stationery = s14;
            product2m3d1.Department = d3;
            product2m3d1.Qty = 250;
            product2m3d1.A_Date = new DateTime(2020, 03, 25);

            product2m3d1.Disbursement = dbur3;
            product2m3d1.Month = 3;
            product2m3d1.Year = 2020;

            DisbursementDetail product3m3d1 = new DisbursementDetail();
            product3m3d1.Stationery = s15;
            product3m3d1.Department = d3;
            product3m3d1.Qty = 180;
            product3m3d1.A_Date = new DateTime(2020, 03, 25);

            product3m3d1.Disbursement = dbur3;
            product3m3d1.Month = 3;
            product3m3d1.Year = 2020;

            dbContext.Add(product1m3d1);
            dbContext.Add(product2m3d1);
            dbContext.Add(product3m3d1);

            //Departmemt 3 , 1 Disbursement, 3 stationery, April

            DisbursementDetail product1m4d1 = new DisbursementDetail();
            product1m4d1.Stationery = s13;
            product1m4d1.Department = d3;
            product1m4d1.Qty = 300;
            product1m4d1.A_Date = new DateTime(2020, 04, 25);

            product1m4d1.Disbursement = dbur4;
            product1m4d1.Month = 4;
            product1m4d1.Year = 2020;

            DisbursementDetail product2m4d1 = new DisbursementDetail();
            product2m4d1.Stationery = s14;
            product2m4d1.Department = d3;
            product2m4d1.Qty = 250;
            product2m4d1.A_Date = new DateTime(2020, 04, 25);

            product2m4d1.Disbursement = dbur4;
            product2m4d1.Month = 4;
            product2m4d1.Year = 2020;

            DisbursementDetail product3m4d1 = new DisbursementDetail();
            product3m4d1.Stationery = s15;
            product3m4d1.Department = d3;
            product3m4d1.Qty = 180;
            product3m4d1.A_Date = new DateTime(2020, 04, 25);

            product3m4d1.Disbursement = dbur4;
            product3m4d1.Month = 4;
            product3m4d1.Year = 2020;

            dbContext.Add(product1m4d1);
            dbContext.Add(product2m4d1);
            dbContext.Add(product3m4d1);


            //Departmemt 3 , 1 Disbursement, 3 stationery, May

            DisbursementDetail product1m5d1 = new DisbursementDetail();
            product1m5d1.Stationery = s13;
            product1m5d1.Department = d3;
            product1m5d1.Qty = 230;
            product1m5d1.A_Date = new DateTime(2020, 05, 25);

            product1m5d1.Disbursement = dbur5;
            product1m5d1.Month = 5;
            product1m5d1.Year = 2020;

            DisbursementDetail product2m5d1 = new DisbursementDetail();
            product2m5d1.Stationery = s14;
            product2m5d1.Department = d3;
            product2m5d1.Qty = 150;
            product2m5d1.A_Date = new DateTime(2020, 05, 25);

            product2m5d1.Disbursement = dbur5;
            product2m5d1.Month = 5;
            product2m5d1.Year = 2020;

            DisbursementDetail product3m5d1 = new DisbursementDetail();
            product3m5d1.Stationery = s15;
            product3m5d1.Department = d3;
            product3m5d1.Qty = 80;
            product3m5d1.A_Date = new DateTime(2020, 05, 25);

            product3m5d1.Disbursement = dbur5;
            product3m5d1.Month = 5;
            product3m5d1.Year = 2020;

            dbContext.Add(product1m5d1);
            dbContext.Add(product2m5d1);
            dbContext.Add(product3m5d1);

            //Departmemt 3 , 1 Disbursement, 2 , June

            DisbursementDetail product1m6d1 = new DisbursementDetail();
            product1m6d1.Stationery = s13;
            product1m6d1.Department = d3;
            product1m6d1.Qty = 200;
            product1m6d1.A_Date = new DateTime(2020, 06, 25);

            product1m6d1.Disbursement = dbur6;
            product1m6d1.Month = 6;
            product1m6d1.Year = 2020;

            DisbursementDetail product2m6d1 = new DisbursementDetail();
            product2m6d1.Stationery = s14;
            product2m6d1.Department = d3;
            product2m6d1.Qty = 150;
            product2m6d1.A_Date = new DateTime(2020, 06, 25);

            product2m6d1.Disbursement = dbur6;
            product2m6d1.Month = 6;
            product2m6d1.Year = 2020;

            DisbursementDetail product3m6d1 = new DisbursementDetail();
            product3m6d1.Stationery = s15;
            product3m6d1.Department = d3;
            product3m6d1.Qty = 0;
            product3m6d1.A_Date = new DateTime(2020, 06, 25);

            product3m6d1.Disbursement = dbur6;
            product3m6d1.Month = 6;
            product3m6d1.Year = 2020;

            dbContext.Add(product1m6d1);
            dbContext.Add(product2m6d1);
            dbContext.Add(product3m6d1);

            //Departmemt 3 , 1 Disbursement, 2, July

            DisbursementDetail product1m7d1 = new DisbursementDetail();
            product1m7d1.Stationery = s13;
            product1m7d1.Department = d3;
            product1m7d1.Qty = 260;
            product1m7d1.A_Date = new DateTime(2020, 07, 25);

            product1m7d1.Disbursement = dbur7;
            product1m7d1.Month = 7;
            product1m7d1.Year = 2020;

            DisbursementDetail product2m7d1 = new DisbursementDetail();
            product2m7d1.Stationery = s14;
            product2m7d1.Department = d3;
            product2m7d1.Qty = 140;
            product2m7d1.A_Date = new DateTime(2020, 07, 25);

            product2m7d1.Disbursement = dbur7;
            product2m7d1.Month = 7;
            product2m7d1.Year = 2020;

            DisbursementDetail product3m7d1 = new DisbursementDetail();
            product3m7d1.Stationery = s15;
            product3m7d1.Department = d3;
            product3m7d1.Qty = 0;
            product3m7d1.A_Date = new DateTime(2020, 07, 25);

            product3m7d1.Disbursement = dbur7;
            product3m7d1.Month = 7;
            product3m7d1.Year = 2020;

            dbContext.Add(product1m7d1);
            dbContext.Add(product2m7d1);
            dbContext.Add(product3m7d1);

            //to use later

            //DisbursementDetail usage53m1 = new DisbursementDetail();
            //usage53m1.Stationery = s3;
            //usage53m1.Department = d2;
            //usage53m1.Qty = 280;
            //usage53m1.A_Date = new DateTime(2020, 01, 25);
            ////usage53m1.Description = "Envelope Brown (3\"x6\")";

            //DisbursementDetail usage53m2 = new DisbursementDetail();
            //usage53m2.Stationery = s3;
            //usage53m2.Department = d2;
            //usage53m2.Qty = 200;
            //usage53m2.A_Date = new DateTime(2020, 02, 25);
            ////usage53m2.Description = "Envelope Brown (3\"x6\")";

            //DisbursementDetail usage53m3 = new DisbursementDetail();
            //usage53m3.Stationery = s3;
            //usage53m3.Department = d2;
            //usage53m3.Qty = 400;
            //usage53m3.A_Date = new DateTime(2020, 03, 25);
            ////usage53m3.Description = "Envelope Brown (3\"x6\")";

            //DisbursementDetail usage63m1 = new DisbursementDetail();
            //usage63m1.Stationery = s3;
            //usage63m1.Department = d1;
            //usage63m1.Qty = 280;
            //usage63m1.A_Date = new DateTime(2020, 01, 25);
            ////usage63m1.Description = "Envelope Brown (3\"x6\")";

            //DisbursementDetail usage63m2 = new DisbursementDetail();
            //usage63m2.Stationery = s3;
            //usage63m2.Department = d1;
            //usage63m2.Qty = 200;
            //usage63m2.A_Date = new DateTime(2020, 02, 25);
            ////usage63m2.Description = "Envelope Brown (3\"x6\")";

            //DisbursementDetail usage63m3 = new DisbursementDetail();
            //usage63m3.Stationery = s3;
            //usage63m3.Department = d1;
            //usage63m3.Qty = 400;
            //usage63m3.A_Date = new DateTime(2020, 03, 25);
            ////usage63m3.Description = "Envelope Brown (3\"x6\")";

            //dbContext.Add(usage43m1);
            //dbContext.Add(usage53m1);
            //dbContext.Add(usage63m1);

            //dbContext.Add(usage43m2);
            //dbContext.Add(usage53m2);
            //dbContext.Add(usage63m2);

            //dbContext.Add(usage43m3);
            //dbContext.Add(usage53m3);
            //dbContext.Add(usage63m3);

            ////2nd product
            //DisbursementDetail p2usage43m1 = new DisbursementDetail();
            //p2usage43m1.Stationery = s1;
            //p2usage43m1.Department = d3;
            //p2usage43m1.Qty = 280;
            //p2usage43m1.A_Date = new DateTime(2020, 01, 25);
            ////p2usage43m1.Description = "Clips Double 1";


            //DisbursementDetail p2usage43m2 = new DisbursementDetail();
            //p2usage43m2.Stationery = s1;
            //p2usage43m2.Department = d3;
            //p2usage43m2.Qty = 200;
            //p2usage43m2.A_Date = new DateTime(2020, 02, 25);
            ////p2usage43m2.Description = "Clips Double 1";

            //DisbursementDetail p2usage43m3 = new DisbursementDetail();
            //p2usage43m3.Stationery = s1;
            //p2usage43m3.Department = d3;
            //p2usage43m3.Qty = 200;
            //p2usage43m3.A_Date = new DateTime(2020, 03, 25);
            ////p2usage43m3.Description = "Clips Double 1";


            //DisbursementDetail p2usage53m1 = new DisbursementDetail();
            //p2usage53m1.Stationery = s1;
            //p2usage53m1.Department = d2;
            //p2usage53m1.Qty = 280;
            //p2usage53m1.A_Date = new DateTime(2020, 01, 25);
            ////p2usage53m1.Description = "Clips Double 1";

            //DisbursementDetail p2usage53m2 = new DisbursementDetail();
            //p2usage53m2.Stationery = s1;
            //p2usage53m2.Department = d2;
            //p2usage53m2.Qty = 200;
            //p2usage53m2.A_Date = new DateTime(2020, 02, 25);
            ////p2usage53m2.Description = "Clips Double 1";


            //DisbursementDetail p2usage53m3 = new DisbursementDetail();
            //p2usage53m3.Stationery = s1;
            //p2usage53m3.Department = d2;
            //p2usage53m3.Qty = 400;
            //p2usage53m3.A_Date = new DateTime(2020, 03, 25);
            ////p2usage53m3.Description = "Clips Double 1";

            //DisbursementDetail p2usage63m1 = new DisbursementDetail();
            //p2usage63m1.Stationery = s1;
            //p2usage63m1.Department = d1;
            //p2usage63m1.Qty = 280;
            //p2usage63m1.A_Date = new DateTime(2020, 01, 25);
            ////p2usage63m1.Description = "Clips Double 1";

            //DisbursementDetail p2usage63m2 = new DisbursementDetail();
            //p2usage63m2.Stationery = s1;
            //p2usage63m2.Department = d1;
            //p2usage63m2.Qty = 200;
            //p2usage63m2.A_Date = new DateTime(2020, 02, 25);
            ////p2usage63m2.Description = "Clips Double 1";

            //DisbursementDetail p2usage63m3 = new DisbursementDetail();
            //p2usage63m3.Stationery = s1;
            //p2usage63m3.Department = d1;
            //p2usage63m3.Qty = 400;
            //p2usage63m3.A_Date = new DateTime(2020, 03, 25);
            ////p2usage63m3.Description = "Clips Double 1";

            //dbContext.Add(p2usage43m1);
            //dbContext.Add(p2usage53m1);
            //dbContext.Add(p2usage63m1);

            //dbContext.Add(p2usage43m2);
            //dbContext.Add(p2usage53m2);
            //dbContext.Add(p2usage63m2);

            //dbContext.Add(p2usage43m3);
            //dbContext.Add(p2usage53m3);
            //dbContext.Add(p2usage63m3);




            DelegatedEmployee de = new DelegatedEmployee();
            de.Employee = e1;
            de.Name = e1.Name;
            de.delegationStatus = DelegationStatus.mock;
            de.EndDate = new DateTime(2020, 1, 1);
            dbContext.Add(de);

            // Saving Changes

            dbContext.SaveChanges();
        }
    }
}
