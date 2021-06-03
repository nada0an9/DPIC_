using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ExpressiveAnnotations.Attributes;
using RequiredIfAttribute = ExpressiveAnnotations.Attributes.RequiredIfAttribute;


namespace WebApplication27.Models
{
    public class recordStage
    {
        [Required(ErrorMessage = "Id is required")]
        public decimal CASE_ID { get; set; }

        public string CASE_TYPE { get; set; }

        [Required(ErrorMessage = "Please Select an option")]
        [MaxLength(20, ErrorMessage = "It cannot exceed 20 characters ")]
        public string ANSWER_GIVEN { get; set; }



        [RequiredIf("ANSWER_GIVEN == 'Not Reachable'", ErrorMessage = "The comment is required")]
        [MaxLength(1000, ErrorMessage = "The comment cannot exceed 1000 characters. ")]
        public string NOT_REACHABLE_COMMENT { get; set; }

        [RequiredIf("ANSWER_GIVEN == 'No'", ErrorMessage = "The receiver ID is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Receiver ID  must be numeric")]
        public Nullable<decimal> RECEIVER_ID { get; set; }





        [RequiredIf("ANSWER_GIVEN == 'No'", ErrorMessage = "The receiver Name is required")]
        [MaxLength(150, ErrorMessage = "The comment cannot exceed 150 characters. ")]
        public string RECEIVER_NAME { get; set; }




        [Required(ErrorMessage = "Please Select an option")]
        [MaxLength(20, ErrorMessage = "It cannot exceed 20 numbers. ")]
        public string CONTACT_ATTEMPT { get; set; }



        [Required(ErrorMessage = "Please Select an option")]
        [MaxLength(3, ErrorMessage = "It cannot exceed 3 numbers. ")]
        public string IS_DIFFER { get; set; }
 



    }
}