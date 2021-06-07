using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RequiredIfAttribute = ExpressiveAnnotations.Attributes.RequiredIfAttribute;

namespace WebApplication27.Models
{
    public class patientInformationStage
    {
        [Required(ErrorMessage = "Id is required")]

        public decimal CASE_ID { get; set; }

        public System.DateTime CASE_START_DATE { get; set; }

        public string CASE_TYPE { get; set; }

        [Required(ErrorMessage = "This Feild is required")]
        [MaxLength(3, ErrorMessage = "The Age cannot exceed 3 characters.")]
        public string patientMRN { get; set; }

        [MaxLength(7, ErrorMessage = "The MRN cannot exceed 7 numbers. ")]
        [MinLength(7, ErrorMessage = "The MRN cannot be less than 7 numbers. ")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "MRN must be numeric")]
        public string MRN { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "The Name cannot exceed 50 characters. ")]
        public string NAME { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [MaxLength(50, ErrorMessage = "The Age cannot exceed 50 characters. ")]
        public string AGE { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [MaxLength(10, ErrorMessage = "The Gender cannot exceed 10 characters. ")]
        public string GENDER { get; set; }

        [RequiredIf("GENDER == 'Female'", ErrorMessage = "pargent is required")]
        [MaxLength(4, ErrorMessage = "The Pregnant cannot exceed 4 characters. ")]
        public string PREGNANT_OR_NOT { get; set; }


        [RequiredIf("PREGNANT_OR_NOT == 'Yes'", ErrorMessage = "Pargent week is required")]
        [MaxLength(50, ErrorMessage = "The Age cannot exceed 50 characters. ")]
        public string PREGNANT_WEEK { get; set; }





        [Required(ErrorMessage = "Allergy  history is required")]
        [MaxLength(1000, ErrorMessage = "The Alergey history  cannot exceed 1000 characters. ")]
        public string ALLERGY_HISTORY { get; set; }



        [Required(ErrorMessage = "Height is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Hight must be numeric")]
        public Nullable<decimal> HEIGHT { get; set; }





        [Required(ErrorMessage = "Weight is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Weight must be numeric")]
        public Nullable<decimal> WEIGHT { get; set; }





        [Required(ErrorMessage = "Diagnosis is required")]
        [MaxLength(1000, ErrorMessage = "The Diagnosis cannot exceed 1000 characters. ")]
        public string DIAGNOSIS { get; set; }





        [MaxLength(1000, ErrorMessage = "The Diagnosis cannot exceed 1000 characters. ")]
        public string ELECTROLYTES { get; set; }






        [Required(ErrorMessage = "Liver function is required")]
        [MaxLength(1000, ErrorMessage = "The liver function cannot exceed 1000 characters. ")]
        public string LIVER_FUNCTION { get; set; }




        [MaxLength(1000, ErrorMessage = "Other relevant laboratory result cannot exceed 1000 characters. ")]
        public string LABORATORY_RESULT { get; set; }






        [Required(ErrorMessage = "Serum creatinine is required")]
        [MaxLength(1000, ErrorMessage = "The Serum creatinine cannot exceed 1000 characters. ")]
        public string SERUM_CREATININE { get; set; }





        [Required(ErrorMessage = "Current / past medications is required")]
        [MaxLength(1000, ErrorMessage = "The Current / past medications cannot exceed 1000 characters. ")]
        public string MEDICATIONS { get; set; }




        [MaxLength(1000, ErrorMessage = "The other related sings and symptoms cannot exceed 1000 characters. ")]
        public string RELATED_SINGS { get; set; }

        public string other { get; set; }

        public List<comorbidities_> ComorbidityList { get; set; }



    }
}