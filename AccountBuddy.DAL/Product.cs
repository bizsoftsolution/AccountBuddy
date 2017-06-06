//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.PurchaseDetails = new HashSet<PurchaseDetail>();
            this.PurchaseOrderDetails = new HashSet<PurchaseOrderDetail>();
            this.PurchaseReturnDetails = new HashSet<PurchaseReturnDetail>();
            this.SalesDetails = new HashSet<SalesDetail>();
            this.SalesOrderDetails = new HashSet<SalesOrderDetail>();
            this.SalesReturnDetails = new HashSet<SalesReturnDetail>();
        }
    
        public int Id { get; set; }
        public Nullable<int> LedgerId { get; set; }
        public string ProductName { get; set; }
        public string ItemCode { get; set; }
        public Nullable<int> AccountGroupId { get; set; }
        public Nullable<int> UOMId { get; set; }
        public Nullable<decimal> PurchaseRate { get; set; }
        public Nullable<decimal> SellingRate { get; set; }
        public Nullable<decimal> MRP { get; set; }
        public Nullable<double> GST { get; set; }
        public Nullable<double> OpeningStock { get; set; }
        public Nullable<double> ReOrderLevel { get; set; }
        public byte[] ProductImage { get; set; }
    
        public virtual AccountGroup AccountGroup { get; set; }
        public virtual Ledger Ledger { get; set; }
        public virtual UOM UOM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseReturnDetail> PurchaseReturnDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesDetail> SalesDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesReturnDetail> SalesReturnDetails { get; set; }
    }
}
