using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        [JsonProperty("config")]
        public JObject Config { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

    }
}
