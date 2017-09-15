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
    
    public partial class StockIn
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StockIn()
        {
            this.StockInDetails = new HashSet<StockInDetail>();
        }
    
        public long Id { get; set; }
        public System.DateTime Date { get; set; }
        public string RefNo { get; set; }
        public string RefCode { get; set; }
        public int LedgerId { get; set; }
        public string Type { get; set; }
        public decimal ItemAmount { get; set; }
        public string Narration { get; set; }
    
        public virtual Ledger Ledger { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockInDetail> StockInDetails { get; set; }
    }
}
