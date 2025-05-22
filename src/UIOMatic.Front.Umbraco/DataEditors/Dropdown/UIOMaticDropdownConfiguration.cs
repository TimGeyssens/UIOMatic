using Umbraco.Cms.Core.PropertyEditors;

namespace UIOMatic.Front.Umbraco.DataEditors.Dropdown;

/// <summary>
/// Configuration for the UIOMatic dropdown editor
/// </summary>
public class UIOMaticDropdownConfiguration
{
    /// <summary>
    /// Gets or sets the type alias of the object to display in the dropdown
    /// </summary>
    [ConfigurationField(
        "typeAlias",
        "Type of object",
        "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.type.html",
        Description = "Select the type of object (reload the page after selecting to populate dropdowns)")]
    public string TypeAlias { get; set; }

    /// <summary>
    /// Gets or sets the value property that holds the value to store
    /// </summary>
    [ConfigurationField(
        "valueColumn",
        "Value property",
        "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.property.html",
        Description = "Property that holds the value you wish to store")]
    public string ValueColumn { get; set; }

    /// <summary>
    /// Gets or sets the text template for displaying items in the dropdown
    /// </summary>
    [ConfigurationField(
        "textTemplate",
        "Text template",
        "textstring",
        Description = "Enter the text template, ie, '{{FirstName}} {{LastName}}'")]
    public string TextTemplate { get; set; }

    /// <summary>
    /// Gets or sets the column to sort on
    /// </summary>
    [ConfigurationField(
        "sortColumn",
        "Column to sort on",
        "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.column.html",
        Description = "Enter the column name to sort on")]
    public string SortColumn { get; set; }

    /// <summary>
    /// Gets or sets the sort order
    /// </summary>
    [ConfigurationField(
        "sortOrder",
        "Sort order",
        "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/picker.tablesortorder.html",
        Description = "Select the sortorder")]
    public string SortOrder { get; set; }
}