using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication27.Models
{
    public class caseDescriptionStage
    {
       

        [RegularExpression("^[0-9]*$", ErrorMessage = "Case ID must be numeric")]
        public decimal CASE_ID { get; set; }
        public System.DateTime CASE_START_DATE { get; set; }


        [Required(ErrorMessage = "Please enter the case description")]
        [MaxLength(1000, ErrorMessage = "The case description cannot exceed 1000 characters. ")]

        public string CASE_DESCRIBTION { get; set; }
        [Required(ErrorMessage = "Requester ID is requierd")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Requester ID must be numeric")]
        public decimal REQUESTER_ID { get; set; }

        [Required(ErrorMessage = "Please select the case type")]
        [MaxLength(30, ErrorMessage = "The case type cannot exceed 30 characters. ")]
        public string CASE_TYPE { get; set; }


        [Required(ErrorMessage = "Please select the case urgency")]
        [MaxLength(50, ErrorMessage = "The case urgency cannot exceed 50 characters. ")]
        public string URGENCY { get; set; }
    }
}