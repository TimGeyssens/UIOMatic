using System;
using UIOmatic.Data;
using UIOMatic.Enums;

namespace UIOMatic.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UIOMaticAttribute : UIOMaticFolderAttribute
    {
        public string ItemIcon { get; set; }

        public UIOMaticRenderType RenderType { get; set; }

        public Type RepositoryType { get; set; }

        public string ConnectionStringName { get; set; }

        public string SortColumn { get; set; }

        public string SortOrder { get; set; }

        public bool ReadOnly { get; set; }

        public UIOMaticAttribute(string name, string alias, string folderIcon, string itemIcon)
            : base(name, alias, folderIcon)
        {
            ItemIcon = itemIcon;
            RenderType = UIOMaticRenderType.Tree;
            SortOrder = "ASC";
            ReadOnly = false;
            RepositoryType = typeof(DefaultUIOMaticRepository);
        }
    }
}