using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIOMatic.Models
{
    public class UIOMaticPagedResult
    {
        public long CurrentPage { get; set; }

        public long ItemsPerPage { get; set; }

        public long TotalPages { get; set; }

        public long TotalItems { get; set; }

        public IEnumerable<object> Items { get; set; }
    }
}