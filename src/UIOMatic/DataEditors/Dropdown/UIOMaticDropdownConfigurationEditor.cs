using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.DataEditors.Dropdown;

public class UIOMaticDropdownConfigurationEditor : ConfigurationEditor<UIOMaticDropdownConfiguration>
{
    public UIOMaticDropdownConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) : base(ioHelper, editorConfigurationParser)
    {
    }
}