//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Barangay_Management_Information_System.Models.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class AuditTrail
    {
        public string AuditTrailId { get; set; }
        public string AccountId { get; set; }
        public string AuditActionsId { get; set; }
        public System.DateTime DateAction { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AuditAction AuditAction { get; set; }
    }
}