using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.Front.Umbraco.DataEditors.ListView;

/// <summary>
/// Data editor for UIOMatic list views
/// </summary>
[DataEditor(
    "UIOMatic.ListView",
    EditorType.PropertyValue,
    "UIOMatic List View",
    "/App_Plugins/UIOMatic/backoffice/views/PropertyEditors/listview.html",
    HideLabel = true)]
public class UIOMaticListViewEditor : DataEditor
{
    private readonly IIOHelper _ioHelper;
    private readonly IEditorConfigurationParser _editorConfigurationParser;

    /// <summary>
    /// Initializes a new instance of the <see cref="UIOMaticListViewEditor"/> class
    /// </summary>
    /// <param name="dataValueEditorFactory">The data value editor factory</param>
    /// <param name="ioHelper">The IO helper</param>
    /// <param name="editorConfigurationParser">The editor configuration parser</param>
    /// <param name="type">The editor type</param>
    public UIOMaticListViewEditor(
        IDataValueEditorFactory dataValueEditorFactory,
        IIOHelper ioHelper,
        IEditorConfigurationParser editorConfigurationParser,
        EditorType type = EditorType.PropertyValue)
        : base(dataValueEditorFactory, type)
    {
        _ioHelper = ioHelper ?? throw new ArgumentNullException(nameof(ioHelper));
        _editorConfigurationParser = editorConfigurationParser ?? throw new ArgumentNullException(nameof(editorConfigurationParser));
    }

    /// <inheritdoc />
    protected override IConfigurationEditor CreateConfigurationEditor()
    {
        return new UIOMaticListViewConfigurationEditor(_ioHelper, _editorConfigurationParser);
    }
}