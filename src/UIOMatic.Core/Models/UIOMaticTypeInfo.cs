using UIOMatic.Core.Enums;

namespace UIOMatic.Core.Models
{
    public class UIOMaticTypeInfo
    {
        public UIOMaticRenderType RenderType { get; set; }

        public string PrimaryKeyColumnName { get; set; }

        public string[] IgnoreColumnsFromListView { get; set; }

        public string NameField { get; set; }

        public bool ReadOnly { get; set; }
    }
}