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
    
    public partial class AdminUser
    {
        public int admin_id { get; set; }
        public string admin_username { get; set; }
        public string admin_password { get; set; }
        public string admin_email { get; set; }
        public string LoginErrorMessage { get; set; }
    }
}
