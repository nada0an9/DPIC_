using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class comorbidityModel
    {
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        public decimal COMORBIDITY_ID { get; set; }
        [Required(ErrorMessage = "Comorbidity Name is required")]
        [MaxLength(220, ErrorMessage = "The Name cannot exceed 220 characters. ")]
        public string COMORBIDITY_NAME { get; set; }
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20, ErrorMessage = "The Status cannot exceed 20 characters. ")]
        public string VISIBILIT_STATUS { get; set; }
        [Required(ErrorMessage = "Display Order is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        [DisplayFormat(DataFormatString = "{0}")]

        public decimal? DISPLAY_ORDER { get; set; }
    }
}