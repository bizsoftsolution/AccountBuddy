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
    
    public partial class Bank
    {
        public int Id { get; set; }
        public string AccountNo { get; set; }
        public string BankAccountName { get; set; }
        public int LedgerId { get; set; }
    
        public virtual Ledger Ledger { get; set; }
    }
}
