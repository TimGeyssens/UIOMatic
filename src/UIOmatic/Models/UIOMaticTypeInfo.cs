using UIOMatic.Enums;

namespace UIOMatic.Models
{
    public class UIOMaticTypeInfo
    {
        public string Alias { get; set; }

        public string Name { get; set; }

        public string TableName { get; set; }

        public string PrimaryKeyColumnName { get; set; }

        public bool AutoIncrementPrimaryKey { get; set; } 

        public UIOMaticRenderType RenderType { get; set; }

        public UIOMaticPropertyInfo[] RawProperties { get; set; }

        public UIOMaticEditablePropertyInfo[] EditableProperties { get; set; }

        public UIOMaticViewablePropertyInfo[] ListViewProperties { get; set; }

        public string NameFieldKey { get; set; }

        public bool ReadOnly { get; set; }
    }
}