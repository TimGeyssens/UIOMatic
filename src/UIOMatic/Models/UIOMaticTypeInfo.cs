using UIOMatic.Enums;

namespace UIOMatic.Models
{
    public class UIOMaticTypeInfo
    {
        public UIOMaticRenderType RenderType { get; set; }

        public string PrimaryKeyColumnName { get; set; }

        public string[] IgnoreColumnsFromListView { get; set; }
    }
}