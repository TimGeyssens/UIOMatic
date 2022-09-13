using UIOMatic.DataEditors.Dropdown;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.DataEditors.ListView;

[DataEditor("UIOMatic.ListView",
    EditorType.PropertyValue,
    "UIOMatic List View",
    "/App_Plugins/UIOMatic/backoffice/views/PropertyEditors/listview.html",
    HideLabel = true)]
public class UIOMaticListViewEditor : DataEditor
{
    private readonly IIOHelper _ioHelper;
    private readonly IEditorConfigurationParser _editorConfigurationParser;

    public UIOMaticListViewEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser, EditorType type = EditorType.PropertyValue) : base(dataValueEditorFactory, type)
    {
        _ioHelper = ioHelper;
        _editorConfigurationParser = editorConfigurationParser;
    }

    protected override IConfigurationEditor CreateConfigurationEditor()
    {
        return new UIOMaticListViewConfigurationEditor(_ioHelper, _editorConfigurationParser);
    }
}