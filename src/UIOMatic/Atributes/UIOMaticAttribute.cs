using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIOMatic.Atributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UIOMaticAttribute : Attribute
    {
        public string Name { get; set; }
        public string   FolderIcon { get; set; }

        public string ItemIcon { get; set; }

        
        public UIOMaticAttribute(string name, string folderIcon, string itemIcon)
        {
            this.Name = name;
            this.FolderIcon = folderIcon;
            this.ItemIcon = itemIcon;
        }
    }
}