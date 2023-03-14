using UIOMatic.DataEditors.DataEditors.Dropdown;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.Front.Umbraco.DataEditors.Dropdown
{
    [DataEditor("UIOMatic.Dropdown",
        EditorType.PropertyValue,
        "UIOMatic Dropdown",
        "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/dropdown.html")]
    public class UIOMaticDropdownEditor : DataEditor
    {
        private readonly IIOHelper _ioHelper;
        private readonly IEditorConfigurationParser _editorConfigurationParser;

        public UIOMaticDropdownEditor(IDataValueEditorFactory dataValueEditorFactory,
            IIOHelper ioHelper,
            IEditorConfigurationParser editorConfigurationParser,
            EditorType type = EditorType.PropertyValue) : base(dataValueEditorFactory, type)
        {
            _ioHelper = ioHelper;
            _editorConfigurationParser = editorConfigurationParser;
        }

        protected override IConfigurationEditor CreateConfigurationEditor()
        {
            return new UIOMaticDropdownConfigurationEditor(_ioHelper, _editorConfigurationParser);
        }
    }
}
