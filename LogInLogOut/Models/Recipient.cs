//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LogInLogOut.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Recipient
    {
        public int RecipientID { get; set; }
        public string DonationIdentifier { get; set; }
        public System.DateTime DateofUse { get; set; }
        public string RelatedCondition { get; set; }
        public string RecipientIdentifier { get; set; }
        public string RecipientCodedName { get; set; }
    
        public virtual Donation Donation { get; set; }
    }
}
