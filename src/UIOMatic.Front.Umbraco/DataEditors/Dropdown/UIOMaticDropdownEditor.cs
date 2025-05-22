using UIOMatic.DataEditors.DataEditors.Dropdown;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.Front.Umbraco.DataEditors.Dropdown
{
    /// <summary>
    /// Data editor for UIOMatic dropdowns
    /// </summary>
    [DataEditor(
        "UIOMatic.Dropdown",
        EditorType.PropertyValue,
        "UIOMatic Dropdown",
        "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/dropdown.html")]
    public class UIOMaticDropdownEditor : DataEditor
    {
        private readonly IIOHelper _ioHelper;
        private readonly IEditorConfigurationParser _editorConfigurationParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIOMaticDropdownEditor"/> class
        /// </summary>
        /// <param name="dataValueEditorFactory">The data value editor factory</param>
        /// <param name="ioHelper">The IO helper</param>
        /// <param name="editorConfigurationParser">The editor configuration parser</param>
        /// <param name="type">The editor type</param>
        public UIOMaticDropdownEditor(
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
            return new UIOMaticDropdownConfigurationEditor(_ioHelper, _editorConfigurationParser);
        }
    }
}
