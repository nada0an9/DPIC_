using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class UsersModel
    {
        [Display(Name = "User")]
        public decimal USER_ID { get; set; }
        [Display(Name = "Full Name ")]
        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(150, ErrorMessage = "The name cannot exceed 150 characters. ")]
        public string FULL_NAME { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(150, ErrorMessage = "The email cannot exceed 150 characters. ")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EMAIL { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(150, ErrorMessage = "Password cannot exceed 150 characters. ")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must must include at least one upper case letter, one lower case letter, and one numeric digit")]
        public string PASSWORD { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("PASSWORD", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [MaxLength(10, ErrorMessage = "The Status cannot exceed 10 characters. ")]
        public string STATUS { get; set; }

        public List<RoleCheckboxViewModel> RoleList { get; set; }

    }


}