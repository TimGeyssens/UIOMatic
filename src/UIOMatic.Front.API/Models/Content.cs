using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using UIOMatic.Front.API.Repositories;
using UIOMatic.Models;

namespace UIOMatic.Front.API.Models
{
    [UIOMatic("Content", "Content", "Content", FolderIcon = "icon-folder", ItemIcon = "icon-document",
    RepositoryType = typeof(FileContentRepository))]
    
    public class Content 
    {
        public int Id { get; set; }

        [UIOMaticField(Name = "Filename", Description = "The name of the file", View = "textfield")]
        [UIOMaticListViewField(Name = "Filename")]
        public string Filename { get; set; }

        [UIOMaticField(Name = "Content", Description = "The content of the file", View = "markdown")]
        public string FileContent { get; set; }
    }
} 