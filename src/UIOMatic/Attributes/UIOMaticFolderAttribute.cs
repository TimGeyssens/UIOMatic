using System;

namespace UIOMatic.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class UIOMaticFolderAttribute : Attribute
    {
        public string Name { get; set; }

        public string Alias { get; set; }

        public string FolderIcon { get; set; }

        public string ParentAlias { get; set; }

        public int Order { get; set; }

        public UIOMaticFolderAttribute(string name, string alias, string folderIcon)
        {
            Name = name;
            Alias = alias;
            FolderIcon = folderIcon;
            ParentAlias = "-1"; // Root
            Order = 0;
        }
    }
}