using System;
using UIOMatic.Core.Enums;

namespace UIOMatic.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UIOMaticAttribute : Attribute
    {
        public string Name { get; set; }
        public string FolderIcon { get; set; }

        public string ItemIcon { get; set; }

        public string SortColumn{ get; set; }

        public string SortOrder { get; set; }
        public UIOMaticRenderType RenderType { get; set; }

        public string ConnectionStringName { get; set; }

        public bool ReadOnly { get; set; }

        public UIOMaticAttribute(string name, string folderIcon, string itemIcon)
        {
            this.Name = name;
            this.FolderIcon = folderIcon;
            this.ItemIcon = itemIcon;

            this.RenderType = UIOMaticRenderType.Tree;
            this.SortOrder = "asc";

            this.ReadOnly = false;
        }
    }
}