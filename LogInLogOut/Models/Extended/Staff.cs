using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LogInLogOut.Models
{
    [MetadataType(typeof(StaffMetaData))]
    public partial class Staff
    {
        public string ConfirmPassword { get; set; }
    }
    public class StaffMetaData
    {
        public int staff_id { get; set; }
        [DisplayName("First name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        public string LastName { get; set; }

        [DisplayName("Enter user email")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "This field is required")]
        public string StaffEmail { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This field is required")]
        [MinLength(6, ErrorMessage = "Minimum 6 charactrs Required")]
        public string StaffPassword { get; set; }

        [DisplayName("Confirm password")]
        [DataType(DataType.Password)]
        [Compare("StaffPassword", ErrorMessage ="Confirm password and password do not match")]
        [Required(ErrorMessage = "This field is required")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Phone number")]
        public string StaffPhoneNr { get; set; }

        public bool IsAdmin { get; set; }
    }
}