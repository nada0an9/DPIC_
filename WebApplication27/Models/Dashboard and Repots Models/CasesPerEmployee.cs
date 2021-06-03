using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication27.Models
{
    public class CasesPerEmployee
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class GroupedUserCountViewModel
    {
        public IReadOnlyList<CasesPerEmployee> Items { get; set; }
        public decimal Total { get; set; }
    }
}