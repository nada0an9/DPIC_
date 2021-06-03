using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class referencesModel
    {
        [Display(Name = "Reference")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        public decimal REFERENCE_ID { get; set; }

        [Display(Name = "Reference Name")]
        [Required(ErrorMessage = "Reference Name is required")]
        [MaxLength(200, ErrorMessage = "The Name cannot exceed 200 characters. ")]
        public string REFERENCE_NAME { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20, ErrorMessage = "The Status cannot exceed 20 characters. ")]
        public string VISIBILIT_STATUS { get; set; }

        [Display(Name = "Display Order")]
        [Required(ErrorMessage = "Display Order is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        [DisplayFormat(DataFormatString = "{0}")]

        public decimal? DISPLAY_ORDER { get; set; }

    }
}