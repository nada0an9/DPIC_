using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace WebApplication27.Models
{
    public class caseDetailsStage
    {
        [Required(ErrorMessage = "Id is required")]
        public decimal CASE_ID { get; set; }

        public string CASE_TYPE { get; set; }

        [MaxLength(1000, ErrorMessage = "The Ultimate Question cannot exceed 1000 characters. ")]


        public string ULTIMATE_QUESTION { get; set; }


        [MaxLength(225, ErrorMessage = "The Ultimate Category cannot exceed 225 characters. ")]
        public string ULTIMATE_CATEGORY { get; set; }


        [MaxLength(225, ErrorMessage = "The Other Ultimate Category cannot exceed 225 characters. ")]
        public string OTHER_ULTIMATE_CATEGORY { get; set; }

    }
}