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
    
    public partial class Product_Spec_Process
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product_Spec_Process()
        {
            this.Product_Spec_Process_Detail = new HashSet<Product_Spec_Process_Detail>();
        }
    
        public long Id { get; set; }
        public System.DateTime Date { get; set; }
        public int ProductId { get; set; }
        public Nullable<int> Qty { get; set; }
    
        public virtual Product Product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Spec_Process_Detail> Product_Spec_Process_Detail { get; set; }
    }
}
