using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.Front.Umbraco.DataEditors.ListView;

public class UIOMaticListViewConfigurationEditor : ConfigurationEditor<UIOMaticListViewConfiguration>
{
    public UIOMaticListViewConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) : base(ioHelper, editorConfigurationParser)
    {
    }
}