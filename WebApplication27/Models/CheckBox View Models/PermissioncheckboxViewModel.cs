using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class PermissioncheckboxViewModel
    {
        public decimal PERMISSION_ID { get; set; }
        public string PERMISSION_NAME { get; set; }
        public string PERMISSION_DESCRIPTION { get; set; }

        public bool Checked { get; set; }


    }
}