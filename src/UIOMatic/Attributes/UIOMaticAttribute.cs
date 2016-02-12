using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public bool IsCanExport { get; set; }

        public bool ReadOnly { get; set; }

        public bool ShowInTree { get; set; }

        public UIOMaticAttribute(string name, string folderIcon, string itemIcon)
        {
            this.Name = name;
            this.FolderIcon = folderIcon;
            this.ItemIcon = itemIcon;

            ShowInTree = true;

            this.RenderType = UIOMaticRenderType.Tree;
            this.SortOrder = "asc";
        }
    }
}