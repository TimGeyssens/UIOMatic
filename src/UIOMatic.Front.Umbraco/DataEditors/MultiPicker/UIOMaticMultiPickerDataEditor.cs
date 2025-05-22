using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.Front.Umbraco.DataEditors.MultiPicker;

/// <summary>
/// Data editor for UIOMatic multi picker
/// </summary>
[DataEditor(
    "UIOMatic.MultiPicker",
    EditorType.PropertyValue,
    "UIOMatic Multi Picker",
    "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/multipicker.html")]
public class UIOMaticMultiPickerDataEditor : DataEditor
{
    private readonly IIOHelper _ioHelper;
    private readonly IEditorConfigurationParser _editorConfigurationParser;

    /// <summary>
    /// Initializes a new instance of the <see cref="UIOMaticMultiPickerDataEditor"/> class
    /// </summary>
    /// <param name="dataValueEditorFactory">The data value editor factory</param>
    /// <param name="ioHelper">The IO helper</param>
    /// <param name="editorConfigurationParser">The editor configuration parser</param>
    /// <param name="type">The editor type</param>
    public UIOMaticMultiPickerDataEditor(
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
        return new UIOMaticMultiPickerConfigurationEditor(_ioHelper, _editorConfigurationParser);
    }
}