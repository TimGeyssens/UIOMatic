using UIOMatic.Attributes;
using UIOMatic.Front.API.Data;

namespace UIOMatic.Front.API.ExampleCode
{
    [UIOMatic("plotterdata", "PlotterData", "PlotterData", FolderIcon = "icon-users", ItemIcon = "icon-user",
      RepositoryType = typeof(IOTPlotterRepository))]
    public class PlotterData
    {
        public int Id { get; set; }

        public string GraphName { get; set; }

        public float Data { get; set; }

    }
}
