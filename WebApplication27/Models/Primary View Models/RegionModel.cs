using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class RegionModel
    {
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        public decimal REGION_ID { get; set; }
        [Required(ErrorMessage = "Region Name is required")]
        [MaxLength(150, ErrorMessage = "The Name cannot exceed 150 characters. ")]
        public string REGION_NAME { get; set; }
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20, ErrorMessage = "The status cannot exceed 20 characters. ")]
        public string Vsibility_status { get; set; }

    }
}