using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class CaseViewModel
    {

        [Required(ErrorMessage = "Id is required")]
        public decimal CASE_ID { get; set; }


        public string CASE_TYPE { get; set; }

        public List<categoryCheckboxViewModel> CategoryList { get; set; }

        public List<QuestionsCheckboxViewModel> QuestionsList { get; set; }

        public List<Case> userCases { get; set; }

        public List<REQUESTER> RequesterList { get; set; }

        public List<PERMISSION> PermissionList { get; set; }

        public IEnumerable<Case> Cases { get; set; }


    }
}