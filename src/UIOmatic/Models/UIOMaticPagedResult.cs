using System.Collections.Generic;
using Newtonsoft.Json;

namespace UIOMatic.Models
{
    public class UIOMaticPagedResult
    {
        [JsonProperty("currentPage")]
        public long CurrentPage { get; set; }

        [JsonProperty("itemsPerPage")]
        public long ItemsPerPage { get; set; }

        [JsonProperty("totalPages")]
        public long TotalPages { get; set; }

        [JsonProperty("totalItems")]
        public long TotalItems { get; set; }

        [JsonProperty("items")]
        public IEnumerable<object> Items { get; set; }
    }
}