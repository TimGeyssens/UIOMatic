using System.Collections.Generic;

namespace UIOMatic.Core.Models
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