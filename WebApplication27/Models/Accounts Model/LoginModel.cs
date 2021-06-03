using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [MaxLength(150, ErrorMessage = "The email cannot exceed 150 characters. ")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EMAIL { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MaxLength(150, ErrorMessage = "The Password cannot exceed 150 characters. ")]
        public string PASSWORD { get; set; }
    }
    public class RegisterModel : LoginModel
    {

        [Display(Name = "Full Name ")]
        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(150, ErrorMessage = "The name cannot exceed 150 characters. ")]
        public string FULL_NAME { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("PASSWORD", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "User Type")]
        [Required(ErrorMessage = "User Type is required")]
        public string User_Type { get; set; }

    }
    public class Profile : LoginModel
    {
   
        [Display(Name = "Full Name ")]
        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(150, ErrorMessage = "The name cannot exceed 150 characters. ")]
        public string FULL_NAME { get; set; }



    }

}