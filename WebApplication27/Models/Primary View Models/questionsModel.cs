using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication27.Models
{
    public class questionsModel
    {
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        [Required(ErrorMessage = "Question is required")]

        public decimal QUESTION_ID { get; set; }

        [Required(ErrorMessage = "Question is required")]
        [MaxLength(250, ErrorMessage = "The Name cannot exceed 250 characters. ")]
        public string QUESTION1 { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        [Display(Name = "Category")]
        public decimal CATEGORY_ID { get; set; }

        [Required(ErrorMessage = "Answer type is required")]
        [MaxLength(50, ErrorMessage = "Answer type cannot exceed 50 characters. ")]
        public string ANSWER_TYPE { get; set; }


        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters. ")]
        public string VISIBILITY_STATUS { get; set; }

        [Required(ErrorMessage = "Required")]
        [MaxLength(20, ErrorMessage = "It cannot exceed 20 characters. ")]
        public string DEFAULT_QUESTION { get; set; }

        [Display(Name = "Display Order")]
        [Required(ErrorMessage = "Display Order is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        [DisplayFormat(DataFormatString = "{0}")]

        public decimal? DISPLAY_ORDER { get; set; }

        public IEnumerable<SelectListItem> CategoriesList { get; set; }

        public virtual CATEGORy CATEGORy { get; set; }

    }
}