using Umbraco.Cms.Core.PropertyEditors;

namespace UIOMatic.Front.Umbraco.DataEditors.MultiPicker;

public class UIOMaticMultiPickerConfiguration
{
    [ConfigurationField("typeAlias", "Type of object", "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.type.html", Description = "Select the type of object (reload the page after selecting to populate dropdowns)")]
    public string TypeAlias { get; set; }

    [ConfigurationField("textTemplate", "Text template", "textstring", Description = "Enter the text template, ie, '{{FirstName}} {{LastName}}'")]
    public string TextTemplate { get; set; }
}