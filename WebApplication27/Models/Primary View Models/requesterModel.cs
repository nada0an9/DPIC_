using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication27.Models
{
    public class requesterModel
    {
        [Display(Name = "Requester ID")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "It must be numeric")]
        [Required(ErrorMessage = "Requester ID is required")]
        [DisplayFormat(DataFormatString = "{0}")]
        public decimal REQUESTER_ID { get; set; }

        [Required(ErrorMessage = "Requester Name is required")]
        [MaxLength(150, ErrorMessage = "The Name cannot exceed 150 characters. ")]
        [Display(Name = "Requester Name")]
        public string REQUESTER_NAME { get; set; }

        [Display(Name = "Contact")]
        [Required(ErrorMessage = "Requester Contact is required")]
        [MaxLength(150, ErrorMessage = "The Contact cannot exceed 150 characters. ")]
        public string CONTACT { get; set; }

        [Display(Name = "Calling From")]

        [Required(ErrorMessage = "Please select one option")]
        [MaxLength(20, ErrorMessage = "Cannot exceed 20 characters. ")]
        public string CALLING_FROM { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public decimal DEPARTMENT_ID { get; set; }



        public IEnumerable<SelectListItem> CityList { get; set; }

        public virtual DEPARTMENT DEPARTMENT { get; set; }

    }
}