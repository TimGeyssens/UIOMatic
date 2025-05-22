using Umbraco.Cms.Core.PropertyEditors;

namespace UIOMatic.Front.Umbraco.DataEditors.MultiPicker;

/// <summary>
/// Configuration for the UIOMatic multi picker data editor
/// </summary>
public class UIOMaticMultiPickerConfiguration
{
    /// <summary>
    /// Gets or sets the type alias of the object to display in the multi picker
    /// </summary>
    [ConfigurationField(
        "typeAlias",
        "Type of object",
        "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.type.html",
        Description = "Select the type of object (reload the page after selecting to populate dropdowns)")]
    public string TypeAlias { get; set; }

    /// <summary>
    /// Gets or sets the text template for displaying items in the multi picker
    /// </summary>
    [ConfigurationField(
        "textTemplate",
        "Text template",
        "textstring",
        Description = "Enter the text template, ie, '{{FirstName}} {{LastName}}'")]
    public string TextTemplate { get; set; }
}