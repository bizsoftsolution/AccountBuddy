﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AccountBuddy.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBFMCGEntities : DbContext
    {
        public DBFMCGEntities()
            : base("name=DBFMCGEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AccountGroup> AccountGroups { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<CompanyDetail> CompanyDetails { get; set; }
        public virtual DbSet<CreditLimitType> CreditLimitTypes { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomFormat> CustomFormats { get; set; }
        public virtual DbSet<DataKeyValue> DataKeyValues { get; set; }
        public virtual DbSet<EntityType> EntityTypes { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<JobOrderIssue> JobOrderIssues { get; set; }
        public virtual DbSet<JobOrderIssueDetail> JobOrderIssueDetails { get; set; }
        public virtual DbSet<JobOrderReceived> JobOrderReceiveds { get; set; }
        public virtual DbSet<JobOrderReceivedDetail> JobOrderReceivedDetails { get; set; }
        public virtual DbSet<JobWorker> JobWorkers { get; set; }
        public virtual DbSet<Journal> Journals { get; set; }
        public virtual DbSet<JournalDetail> JournalDetails { get; set; }
        public virtual DbSet<Ledger> Ledgers { get; set; }
        public virtual DbSet<LogDetail> LogDetails { get; set; }
        public virtual DbSet<LogDetailType> LogDetailTypes { get; set; }
        public virtual DbSet<LogMaster> LogMasters { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductDetail> ProductDetails { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual DbSet<PurchaseReturn> PurchaseReturns { get; set; }
        public virtual DbSet<PurchaseReturnDetail> PurchaseReturnDetails { get; set; }
        public virtual DbSet<Receipt> Receipts { get; set; }
        public virtual DbSet<ReceiptDetail> ReceiptDetails { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SalesDetail> SalesDetails { get; set; }
        public virtual DbSet<SalesOrder> SalesOrders { get; set; }
        public virtual DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        public virtual DbSet<SalesReturn> SalesReturns { get; set; }
        public virtual DbSet<SalesReturnDetail> SalesReturnDetails { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<StateDetail> StateDetails { get; set; }
        public virtual DbSet<StockGroup> StockGroups { get; set; }
        public virtual DbSet<StockIn> StockIns { get; set; }
        public virtual DbSet<StockInDetail> StockInDetails { get; set; }
        public virtual DbSet<StockInProcess> StockInProcesses { get; set; }
        public virtual DbSet<StockInProcessDetail> StockInProcessDetails { get; set; }
        public virtual DbSet<StockOut> StockOuts { get; set; }
        public virtual DbSet<StockOutDetail> StockOutDetails { get; set; }
        public virtual DbSet<StockSeparated> StockSeparateds { get; set; }
        public virtual DbSet<StockSeperatedDetail> StockSeperatedDetails { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<TransactionType> TransactionTypes { get; set; }
        public virtual DbSet<UOM> UOMs { get; set; }
        public virtual DbSet<UserAccount> UserAccounts { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<UserTypeDetail> UserTypeDetails { get; set; }
        public virtual DbSet<UserTypeFormDetail> UserTypeFormDetails { get; set; }
    }
}
