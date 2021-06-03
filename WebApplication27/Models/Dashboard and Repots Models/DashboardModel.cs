using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class DashboardModel
    {
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Start Date")]

        public DateTime Start_date { get; set; }
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "End Date")]
        public DateTime End_date { get; set; }
        [Required(ErrorMessage = "Option is required")]
        public string ChartType { get; set; }

    }
}