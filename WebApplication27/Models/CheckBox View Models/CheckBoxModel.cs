using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class CheckBoxModel
    {
        public decimal id { get; set; }
        public string name { get; set; }
        public bool Checked { get; set; }


    }
    public class references_ : CheckBoxModel
    {
        public string other { get; set; }
    }
    public class comorbidities_ : CheckBoxModel
    {
        public string other { get; set; }


    }
    public class categories_ : CheckBoxModel
    {
        public string defult { get; set; }

    }
    public class questions_ : CheckBoxModel
    {
        public string Answer { get; set; }
        public string defult { get; set; }
        public string fieldType { get; set; }
        public string Category { get; set; }
        public Nullable<decimal> Choice_Id { get; set; }
        public List<MULTIPLE_CHOICE> Choices { set; get; }

    }
    public class permissions_ : CheckBoxModel
    {
        public string description { get; set; }


    }
    public class roles_ : CheckBoxModel
    {
        public string description { get; set; }


    }




}