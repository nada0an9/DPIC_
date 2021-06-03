using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RequiredIfAttribute = ExpressiveAnnotations.Attributes.RequiredIfAttribute;


namespace WebApplication27.Models
{
    public class ReportModel
    {

        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Start Date")]
        public DateTime Start_date { get; set; }
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Start Date")]
        public DateTime End_date { get; set; }
        [Required(ErrorMessage = "please select one option")]
        public bool allUserOrOne { get; set; }

        [Required(ErrorMessage = "please select one user")]
        [Display(Name = "User")]
        public decimal USER_ID { get; set; }

        public virtual USER user { get; set; }





    }
}