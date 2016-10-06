using System;
using UIOMatic.Enums;

namespace UIOMatic.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UIOMaticAttribute : UIOMaticFolderAttribute
    {
        public string ItemIcon { get; set; }

        public string SortColumn { get; set; }

        public string SortOrder { get; set; }

        public UIOMaticRenderType RenderType { get; set; }

        public string ConnectionStringName { get; set; }

        public bool ReadOnly { get; set; }

        [Obsolete("Use the constructor where you provide an alias for this type", false)]
        public UIOMaticAttribute(string name, string folderIcon, string itemIcon)
            : this(name, "", folderIcon, itemIcon)
        { }

        public UIOMaticAttribute(string name, string alias, string folderIcon, string itemIcon)
            : base(name, alias, folderIcon)
        {
            ItemIcon = itemIcon;
            RenderType = UIOMaticRenderType.Tree;
            SortOrder = "asc";
            ReadOnly = false;
        }
    }
}