//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication27.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CASE_QUESTIONS
    {
        public decimal CASE_ID { get; set; }
        public decimal QUESTION_ID { get; set; }
        public string QUESTION_NAME { get; set; }
        public string ANSWER { get; set; }
        public Nullable<decimal> CHOICE_ID { get; set; }
    
        public virtual Case Case { get; set; }
        public virtual MULTIPLE_CHOICE MULTIPLE_CHOICE { get; set; }
        public virtual QUESTION QUESTION { get; set; }
    }
}
