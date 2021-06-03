using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class categoryCheckboxViewModel
    {
        public decimal id { get; set; }
        public string name { get; set; }
        public bool Checked { get; set; }
        public string defult { get; set; }
    }
}