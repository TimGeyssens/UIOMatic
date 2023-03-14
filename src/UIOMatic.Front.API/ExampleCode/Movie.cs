using System.ComponentModel.DataAnnotations;
using UIOMatic.Attributes;
using UIOMatic.Front.API.Data;

namespace UIOMatic.Front.API.ExampleCode
{
    [UIOMatic("movies", "Movies", "Movie", FolderIcon = "icon-movie", ItemIcon = "icon-movie", RenderType = Enums.UIOMaticRenderType.Tree,
       RepositoryType = typeof(MovieRepository))]

    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [UIOMaticListViewField]
        [UIOMaticField(Name = "Title", Description = "Enter the movie title")]
        public string Title { get; set; }

        [Required]
        [UIOMaticListViewField]
        [UIOMaticField(Name = "Genre", Description = "Enter the movie genre")]
        public string Genre { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
