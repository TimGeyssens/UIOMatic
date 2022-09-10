using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UIOMatic.Models
{
    public class UIOMaticEditablePropertyInfo : UIOMaticPropertyInfo
    {
        [JsonProperty("tab")]
        public string Tab { get; set; }

        [JsonProperty("tabOrder")]
        public int TabOrder { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("view")]
        public string View { get; set; }

        [JsonProperty("config")]
        public JObject Config { get; set; }

        public UIOMaticEditablePropertyInfo()
        {
            Tab = "General";
        }
    }

    public class UIOMaticViewablePropertyInfo : UIOMaticPropertyInfo
    {
        [JsonProperty("view")]
        public string View { get; set; }

        [JsonProperty("config")]
        public JObject Config { get; set; }
    }

    public class UIOMaticFilterPropertyInfo : UIOMaticPropertyInfo
    {
        [JsonProperty("keyPropertyName")]
        public string KeyPropertyName { get; set; }

        [JsonProperty("keyColumnName")]
        public string KeyColumnName { get; set; }

        [JsonProperty("view")]
        public string View { get; set; }

        [JsonProperty("config")]
        public JObject Config { get; set; }
    }

    public class UIOMaticPropertyInfo
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("label")]
        public string Name { get; set; }

        [JsonProperty("columnName")]
        public string ColumnName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }
    }
}