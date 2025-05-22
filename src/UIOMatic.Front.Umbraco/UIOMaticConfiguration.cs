using Microsoft.Extensions.Configuration;

namespace UIOMatic.Front.Umbraco
{
    public class UIOMaticConfiguration
    {
        public const string SectionName = "UIOMatic";

        public int DefaultListViewPageSize { get; set; } = 10;

        public string RteFieldEditorButtons { get; set; } =
            "[\"preview\", \"|\", \"undo\", \"redo\", \"|\", \"copy\", \"cut\", \"paste\", \"|\", \"bold\", \"italic\", \"|\", \"link\", \"unlink\"]";
    }
}
