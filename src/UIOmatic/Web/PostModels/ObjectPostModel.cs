using System.Dynamic;

namespace UIOmatic.Web.PostModels
{
    public class ObjectPostModel
    {
        public string TypeAlias { get; set; }

        public ExpandoObject Value { get; set; }
    }
}
