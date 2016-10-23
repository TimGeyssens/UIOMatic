using System;
using UIOMatic.Data;
using UIOMatic.Enums;

namespace UIOMatic.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UIOMaticAttribute : UIOMaticFolderAttribute
    {
        public string ItemName { get; set; }

        public string ItemIcon { get; set; }

        public UIOMaticRenderType RenderType { get; set; }

        public Type RepositoryType { get; set; }

        public string ConnectionStringName { get; set; }

        public string SortColumn { get; set; }

        public string SortOrder { get; set; }

        public bool ReadOnly { get; set; }

        public UIOMaticAttribute(string alias, string folderName, string itemName, string folderIcon, string itemIcon)
            : base(alias, folderName, folderIcon)
        {
            ItemName = itemName;
            ItemIcon = itemIcon;
            RenderType = UIOMaticRenderType.Tree;
            SortOrder = "ASC";
            ReadOnly = false;
            RepositoryType = typeof(DefaultUIOMaticRepository);
        }
    }
}