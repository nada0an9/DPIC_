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
    
    public partial class DEPARTMENT
    {
        public DEPARTMENT()
        {
            this.REQUESTERS = new HashSet<REQUESTER>();
        }
    
        public decimal DEPARTMENT_ID { get; set; }
        public string COST_CODE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string VSIBILITY_STATUS { get; set; }
        public decimal REGION_ID { get; set; }
        public Nullable<decimal> CREATED_BY { get; set; }
        public Nullable<System.DateTime> CHANGED_DATE { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
    
        public virtual REGION REGION { get; set; }
        public virtual USER USER { get; set; }
        public virtual ICollection<REQUESTER> REQUESTERS { get; set; }
    }
}
