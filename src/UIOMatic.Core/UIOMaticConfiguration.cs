namespace UIOMatic
{
    public class UIOMaticConfiguration
    {
        public int DefaultListViewPageSize { get; set; } = 10;

        public string RteFieldEditorButtons { get; set; } =
            "[\"preview\", \"|\", \"undo\", \"redo\", \"|\", \"copy\", \"cut\", \"paste\", \"|\", \"bold\", \"italic\", \"|\", \"link\", \"unlink\"]";
    }
}
