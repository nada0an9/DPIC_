using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace WebApplication27.Models
{
    public class QuestionsCheckboxViewModel
    {

        public decimal id { get; set; }
        public string name { get; set; }
        public string Answer { get; set; }
        public string defult { get; set; }
        public string fieldType { get; set; }
        public string Category { get; set; }
        public bool Checked { get; set; }
        public Nullable<decimal> Choice_Id { get; set; }

        public List<MULTIPLE_CHOICE> Choices { set; get; }


    }
}