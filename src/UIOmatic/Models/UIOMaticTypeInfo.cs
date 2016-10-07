using UIOMatic.Enums;

namespace UIOMatic.Models
{
    public class UIOMaticTypeInfo
    {
        public string TypeAlias { get; set; }

        public UIOMaticRenderType RenderType { get; set; }

        public string PrimaryKeyColumnName { get; set; }

        public string[] IgnoreColumnsFromListView { get; set; }

        public string NameField { get; set; }

        public bool ReadOnly { get; set; }
    }
}