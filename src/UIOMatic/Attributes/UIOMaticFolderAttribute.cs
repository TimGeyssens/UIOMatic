using System;

namespace UIOMatic.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class UIOMaticFolderAttribute : Attribute
    {
        public string Alias { get; set; }

        public string FolderName { get; set; }

        public string FolderIcon { get; set; }

        public string ParentAlias { get; set; }

        public int Order { get; set; }

        public UIOMaticFolderAttribute(string alias, string folderName)
        {
            Alias = alias;
            FolderName = folderName;
            FolderIcon = "icon-folder";
            ParentAlias = "-1"; // Root
            Order = 0;
        }
    }
}