using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIOMatic.Models
{
    public class UIOMaticActionInfo
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("view")]
        public string View { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

    }
}
