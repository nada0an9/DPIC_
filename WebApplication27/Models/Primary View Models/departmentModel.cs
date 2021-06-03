using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class departmentModel
    {
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        [Display(Name = "Department")]
        public decimal DEPARTMENT_ID { get; set; }

        [Required(ErrorMessage = "Cost Code is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = " must be numeric")]
        [DisplayFormat(DataFormatString = "{0}")]
        [MaxLength(150, ErrorMessage = "It cannot exceed 150 characters. ")]

        public string COST_CODE { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(150, ErrorMessage = "The Name cannot exceed 150 characters. ")]
        public string DEPARTMENT_NAME { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [MaxLength(10, ErrorMessage = "The Status cannot exceed 10 characters. ")]
        public string VSIBILITY_STATUS { get; set; }

        [Required(ErrorMessage = "Region is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        [Display(Name = "Region")]
        public decimal REGION_ID { get; set; }


    }
}