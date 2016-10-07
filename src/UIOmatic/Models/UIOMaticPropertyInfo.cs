using Newtonsoft.Json.Linq;

namespace UIOMatic.Models
{
    public class UIOMaticEditablePropertyInfo : UIOMaticViewablePropertyInfo
    {
        public string Tab { get; set; }

        public string Description { get; set; }

        public UIOMaticEditablePropertyInfo()
        {
            Tab = "Misc";
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