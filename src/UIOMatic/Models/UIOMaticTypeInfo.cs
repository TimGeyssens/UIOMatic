using UIOMatic.Enums;

namespace UIOMatic.Models
{
    public class UIOMaticTypeInfo
    {
        public UIOMaticRenderType RenderType { get; set; }

        public string PrimaryKeyColumnName { get; set; }

        public string[] IgnoreColumnsFromListView { get; set; }

        public string NameField { get; set; }

        public string DisplayName { get; set; }

        public string QueryTemplate { get; set; }

        public bool IsCanExport { get; set; }

        public bool IsReadOnly { get; set; }

        public bool ShowInTree { get; set; }
    }
}