using Newtonsoft.Json.Linq;

namespace UIOMatic.Models
{
    public class UIOMaticEditablePropertyInfo : UIOMaticPropertyInfo
    {
        public string Tab { get; set; }

        public string Description { get; set; }

        public string View { get; set; }

        public JObject Config { get; set; }

        public UIOMaticEditablePropertyInfo()
        {
            Tab = "General";
        }
    }

    public class UIOMaticViewablePropertyInfo : UIOMaticPropertyInfo
    {
        public string View { get; set; }

        public JObject Config { get; set; }
    }

    public class UIOMaticPropertyInfo
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
    }
}