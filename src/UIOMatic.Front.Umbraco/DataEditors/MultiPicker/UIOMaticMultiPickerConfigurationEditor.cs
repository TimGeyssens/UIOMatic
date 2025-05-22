using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.Front.Umbraco.DataEditors.MultiPicker;

/// <summary>
/// Configuration editor for the UIOMatic multi picker data editor
/// </summary>
public class UIOMaticMultiPickerConfigurationEditor : ConfigurationEditor<UIOMaticMultiPickerConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UIOMaticMultiPickerConfigurationEditor"/> class
    /// </summary>
    /// <param name="ioHelper">The IO helper</param>
    /// <param name="editorConfigurationParser">The editor configuration parser</param>
    public UIOMaticMultiPickerConfigurationEditor(
        IIOHelper ioHelper,
        IEditorConfigurationParser editorConfigurationParser)
        : base(ioHelper, editorConfigurationParser)
    {
    }
}