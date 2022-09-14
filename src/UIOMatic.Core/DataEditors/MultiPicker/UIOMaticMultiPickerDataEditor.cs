using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.DataEditors.MultiPicker;

[DataEditor("UIOMatic.MultiPicker",
    EditorType.PropertyValue,
    "UIOMatic Multi Picker",
    "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/multipicker.html")]
public class UIOMaticMultiPickerDataEditor : DataEditor
{
    private readonly IIOHelper _ioHelper;
    private readonly IEditorConfigurationParser _editorConfigurationParser;

    public UIOMaticMultiPickerDataEditor(IDataValueEditorFactory dataValueEditorFactory,
        IIOHelper ioHelper,
        IEditorConfigurationParser editorConfigurationParser,
        EditorType type = EditorType.PropertyValue) : base(dataValueEditorFactory, type)
    {
        _ioHelper = ioHelper;
        _editorConfigurationParser = editorConfigurationParser;
    }

    protected override IConfigurationEditor CreateConfigurationEditor()
    {
        return new UIOMaticMultiPickerConfigurationEditor(_ioHelper, _editorConfigurationParser);
    }
}