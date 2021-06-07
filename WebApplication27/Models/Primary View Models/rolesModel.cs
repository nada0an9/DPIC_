using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class rolesModel
    {
        public decimal ROLE_ID { get; set; }

        [Required(ErrorMessage = "name must be filled out")]
        [MaxLength(20, ErrorMessage = "Cannot exceed 20 characters. ")]
        public string ROLE_NAME { get; set; }

        public List<permissions_> PermissionList { get; set; }


    }
}