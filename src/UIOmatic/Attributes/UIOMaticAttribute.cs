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
        
        public UIOMaticAttribute(string name, string folderIcon, string itemIcon)
            : base(name, Guid.NewGuid().ToString(), folderIcon) // Alias isn't important as we'll use the assembly qualified name anyways in the tree
        {
            ItemIcon = itemIcon;
            RenderType = UIOMaticRenderType.Tree;
            SortOrder = "asc";
            ReadOnly = false;
        }
    }
}