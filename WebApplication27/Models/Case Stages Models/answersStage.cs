using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace WebApplication27.Models
{
    public class answersStage
    {
        [Required(ErrorMessage = "Id is required")]
        public decimal CASE_ID { get; set; }

        public string CASE_TYPE { get; set; }

        [Required(ErrorMessage = "Please enter the case answer")]
        [MaxLength(1000, ErrorMessage = "The case answer cannot exceed 1000 characters. ")]
        public string ANSWER { get; set; }
        public string RESEARCHER_NAME { get; set; }



        public List<CheckBoxModel> RefrensesList { get; set; }

    }
}