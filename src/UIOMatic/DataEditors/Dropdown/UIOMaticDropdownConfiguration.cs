using Umbraco.Cms.Core.PropertyEditors;

namespace UIOMatic.DataEditors.Dropdown;

public class UIOMaticDropdownConfiguration
{
    [ConfigurationField("typeAlias", "Type of object", "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.type.html", Description = "Select the type of object (reload the page after selecting to populate dropdowns)")]
    public string TypeAlias { get; set; }

    [ConfigurationField("valueColumn", "Value property", "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.property.html", Description = "Property that holds the value you wish to store")]
    public string ValueColumn { get; set; }

    [ConfigurationField("textTemplate", "Text template", "textstring", Description = "Enter the text template, ie, '{{FirstName}} {{LastName}}'")]
    public string TextTemplate { get; set; }

    [ConfigurationField("sortColumn", "Column to sort on", "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.column.html", Description = "Enter the column name to sort on")]
    public string SortColumn { get; set; }

    [ConfigurationField("sortOrder", "Sort order", "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/picker.tablesortorder.html", Description = "Select the sortorder")]
    public string SortOrder { get; set; }
}