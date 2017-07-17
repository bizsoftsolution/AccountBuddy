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
    
    public partial class JobOrderReceived
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public JobOrderReceived()
        {
            this.JobOrderReceivedDetails = new HashSet<JobOrderReceivedDetail>();
        }
    
        public long Id { get; set; }
        public System.DateTime JRDate { get; set; }
        public string RefNo { get; set; }
        public string RefCode { get; set; }
        public int JobWorkerId { get; set; }
        public decimal ItemAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal ExtraAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Narration { get; set; }
    
        public virtual JobWorker JobWorker { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobOrderReceivedDetail> JobOrderReceivedDetails { get; set; }
    }
}
