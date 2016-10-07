using UIOMatic.Enums;

namespace UIOMatic.Models
{
    public class UIOMaticTypeInfo
    {
        public string TypeAlias { get; set; }

        public string TableName { get; set; }

        public string PrimaryKeyColumnName { get; set; }

        public bool AutoIncrementPrimaryKey { get; set; } 

        public UIOMaticRenderType RenderType { get; set; }

        public UIOMaticPropertyInfo[] Properties { get; set; }

        public UIOMaticPropertyInfo[] ListViewProperties { get; set; }

        public string NameField { get; set; }

        public bool ReadOnly { get; set; }
    }
}