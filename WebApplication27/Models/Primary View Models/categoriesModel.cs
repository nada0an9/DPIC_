using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class categoriesModel
    {

        [RegularExpression("^[0-9]*$", ErrorMessage = "MRN must be numeric")]
        [Display(Name = "Category")]
        public decimal CATEGORY_ID { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(225, ErrorMessage = "Cannot exceed 255 characters ")]
        public string CATEGORY_NAME { get; set; }

        [Required(ErrorMessage = "Category Type is required")]
        [MaxLength(50, ErrorMessage = "Cannot exceed 50 characters ")]

        public string CATEGORY_TYPE { get; set; }
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(10, ErrorMessage = "Cannot exceed 10 characters ")]
        public string VSIBILITY_STATUS { get; set; }
        [Required(ErrorMessage = "This field is Required")]
        [MaxLength(3, ErrorMessage = "Cannot exceed 3 characters ")]
        public string DEFUALT { get; set; }

        [Required(ErrorMessage = "Display order is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Display order must be numeric")]
        [DisplayFormat(DataFormatString = "{0}")]
        [Display(Name = "Display order")]

        public decimal? DISPLAY_ORDER { get; set; }

    }
}