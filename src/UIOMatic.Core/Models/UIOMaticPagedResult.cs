using System.Collections.Generic;
using Newtonsoft.Json;

namespace UIOMatic.Models
{
    public class UIOMaticPagedResult : UIOMaticPagedResult<object>
    { }

    public class UIOMaticPagedResult<TEntity>
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
        public IEnumerable<TEntity> Items { get; set; }
    }
}