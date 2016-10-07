using Newtonsoft.Json.Linq;

namespace UIOMatic.Models
{
    public class UIOMaticPropertyInfo
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Tab { get; set; }

        public string Description { get; set; }

        public string View { get; set; }

        public string Type { get; set; }

        public JObject Config { get; set; }

        public UIOMaticPropertyInfo()
        {
            Tab = "Misc";
        }
    }
}