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
    
    public partial class AppConnectionLoginSucceed
    {
        public long Id { get; set; }
        public Nullable<long> AppConnectionId { get; set; }
        public Nullable<int> LoginId { get; set; }
        public Nullable<System.DateTime> SucceedAt { get; set; }
        public string Type { get; set; }
    
        public virtual AppConnection AppConnection { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}
