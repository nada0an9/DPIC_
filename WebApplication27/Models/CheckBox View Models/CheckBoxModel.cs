using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExpressiveAnnotations.Attributes;
using System.ComponentModel.DataAnnotations;
using RequiredIfAttribute = ExpressiveAnnotations.Attributes.RequiredIfAttribute;

namespace WebApplication27.Models
{
    public class CheckBoxModel
    {
        public decimal id { get; set; }
        public string name { get; set; }

        [RequiredIf("name == 'Other' && Checked == true" , ErrorMessage = "Other is required")]
        public string other { get; set; }
        public bool Checked { get; set; }

    }
}