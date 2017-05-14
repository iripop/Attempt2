using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LogInLogOut.Models.Extended
{
    [MetadataType(typeof(AdminUserMetaData))]
    public class AdminUser
    {
        public string LoginErrorMessage { get; set; }
    }
    public class AdminUserMetaData
    {
        public int admin_id { get; set; }
        [DisplayName("Username")]
        [Required(ErrorMessage = "This field is required")]
        public string admin_username { get; set; }
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This field is required")]
        public string admin_password { get; set; }
        public string admin_email { get; set; }
        public string LoginErrorMessage { get; set; }
    }

}