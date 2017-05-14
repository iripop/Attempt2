using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace LogInLogOut.Models
{
    public class AdminLogin
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email address required")]
        [DisplayName("Email address")]
        public string AdminEmail { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DisplayName("Password")]
        public string AdminPassword { get; set; }

        [DisplayName("Remember me")]
        public bool RememberMe { get; set; }

        public string LoginErrorMessage { get; set; }
    }
}