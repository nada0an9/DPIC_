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
    
    public partial class CATEGORy
    {
        public CATEGORy()
        {
            this.CASE_CATEGORIES = new HashSet<CASE_CATEGORIES>();
            this.QUESTIONS = new HashSet<QUESTION>();
        }
    
        public decimal CATEGORY_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string CATEGORY_TYPE { get; set; }
        public string VSIBILITY_STATUS { get; set; }
        public string DEFUALT { get; set; }
        public Nullable<decimal> DISPLAY_ORDER { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<decimal> CREATED_BY { get; set; }
        public Nullable<System.DateTime> CHANGED_DATE { get; set; }
    
        public virtual ICollection<CASE_CATEGORIES> CASE_CATEGORIES { get; set; }
        public virtual USER USER { get; set; }
        public virtual ICollection<QUESTION> QUESTIONS { get; set; }
    }
}