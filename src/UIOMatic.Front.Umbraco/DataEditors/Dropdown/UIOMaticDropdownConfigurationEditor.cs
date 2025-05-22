using UIOMatic.Front.Umbraco.DataEditors.Dropdown;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.DataEditors.DataEditors.Dropdown;

/// <summary>
/// Configuration editor for the UIOMatic dropdown editor
/// </summary>
public class UIOMaticDropdownConfigurationEditor : ConfigurationEditor<UIOMaticDropdownConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UIOMaticDropdownConfigurationEditor"/> class
    /// </summary>
    /// <param name="ioHelper">The IO helper</param>
    /// <param name="editorConfigurationParser">The editor configuration parser</param>
    public UIOMaticDropdownConfigurationEditor(
        IIOHelper ioHelper,
        IEditorConfigurationParser editorConfigurationParser)
        : base(ioHelper, editorConfigurationParser)
    {
    }
}