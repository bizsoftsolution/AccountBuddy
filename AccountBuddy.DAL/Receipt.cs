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
    
    public partial class Receipt
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Receipt()
        {
            this.ReceiptDetails = new HashSet<ReceiptDetail>();
        }
    
        public long Id { get; set; }
        public string EntryNo { get; set; }
        public System.DateTime ReceiptDate { get; set; }
        public int LedgerId { get; set; }
        public string ReceiptMode { get; set; }
        public decimal Amount { get; set; }
        public string Particulars { get; set; }
        public string RefNo { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> Extracharge { get; set; }
        public string ChequeNo { get; set; }
        public Nullable<System.DateTime> ChequeDate { get; set; }
        public Nullable<System.DateTime> CleareDate { get; set; }
        public string ReceivedFrom { get; set; }
        public string VoucherNo { get; set; }
    
        public virtual Ledger Ledger { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReceiptDetail> ReceiptDetails { get; set; }
    }
}
