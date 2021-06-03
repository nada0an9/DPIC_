using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class permissions
    {
        public decimal PERMISSION_ID { get; set; }
        public string PERMISSION_NAME { get; set; }
        public string CONTROLLER_NAME { get; set; }
        public string ACTION_NAME { get; set; }
        public string TYPE { get; set; }

        public List<PERMISSION> PermissionList { get; set; }
        public List<ROLE> RoleList { get; set; }

    }
}