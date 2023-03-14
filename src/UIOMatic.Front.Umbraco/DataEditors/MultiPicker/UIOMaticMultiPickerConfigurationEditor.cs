using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.Front.Umbraco.DataEditors.MultiPicker;

public class UIOMaticMultiPickerConfigurationEditor : ConfigurationEditor<UIOMaticMultiPickerConfiguration>
{
    public UIOMaticMultiPickerConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) : base(ioHelper, editorConfigurationParser)
    {
    }
}