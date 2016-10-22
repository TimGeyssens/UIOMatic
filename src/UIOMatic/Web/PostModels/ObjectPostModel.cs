using System.Dynamic;

namespace UIOMatic.Web.PostModels
{
    public class ObjectPostModel
    {
        public string TypeAlias { get; set; }

        public ExpandoObject Value { get; set; }
    }
}
