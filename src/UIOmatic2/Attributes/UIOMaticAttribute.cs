using System;
using UIOMatic.Enums;

namespace UIOMatic.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UIOMaticAttribute : Attribute
    {
        public string Name { get; set; }

        public string FolderIcon { get; set; }

        public string ItemIcon { get; set; }

        public string SortColumn { get; set; }

        public string SortOrder { get; set; }

        public UIOMaticRenderType RenderType { get; set; }

        public string ConnectionStringName { get; set; }

        public bool ReadOnly { get; set; }

        public UIOMaticAttribute(string name, string folderIcon, string itemIcon)
        {
            Name = name;

            FolderIcon = folderIcon;

            ItemIcon = itemIcon;

            RenderType = UIOMaticRenderType.Tree;

            SortOrder = "asc";

            ReadOnly = false;
        }
    }
}