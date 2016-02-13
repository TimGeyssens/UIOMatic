using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIOMatic.Models
{
    public class UIOMaticQueryInfo
    {
        public string TypeName { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public string SearchTerm { get; set; }
        public int ItemsPerPage { get; set; }
        public int PageNumber { get; set; } 
        public IEnumerable<UIOMaticFilterPropertyInfo> FilterProperty { get; set; }
    }
}