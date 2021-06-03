using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class choiceModel
    {
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        public decimal CHOICE_ID { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(150, ErrorMessage = "The Name cannot exceed 150 characters. ")]
        public string CHOICE_NAM { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        [Required(ErrorMessage = "Question is required")]
        public decimal QUESTION_ID { get; set; }

        [Required(ErrorMessage = "Display Order is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        [DisplayFormat(DataFormatString = "{0}")]

        public decimal? DISPLAY_ORDER { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters. ")]
        public string VISIBILITY_STATUS { get; set; }



        public virtual QUESTION QUESTION { get; set; }
    }
}