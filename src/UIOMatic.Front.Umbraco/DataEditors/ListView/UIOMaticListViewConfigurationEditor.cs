using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.Front.Umbraco.DataEditors.ListView;

/// <summary>
/// Configuration editor for the UIOMatic list view editor
/// </summary>
public class UIOMaticListViewConfigurationEditor : ConfigurationEditor<UIOMaticListViewConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UIOMaticListViewConfigurationEditor"/> class
    /// </summary>
    /// <param name="ioHelper">The IO helper</param>
    /// <param name="editorConfigurationParser">The editor configuration parser</param>
    public UIOMaticListViewConfigurationEditor(
        IIOHelper ioHelper,
        IEditorConfigurationParser editorConfigurationParser)
        : base(ioHelper, editorConfigurationParser)
    {
    }
}