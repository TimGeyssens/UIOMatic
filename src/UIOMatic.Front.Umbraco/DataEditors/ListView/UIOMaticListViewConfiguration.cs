using Umbraco.Cms.Core.PropertyEditors;

namespace UIOMatic.Front.Umbraco.DataEditors.ListView;

public class UIOMaticListViewConfiguration
{
    [ConfigurationField("typeAlias", "Type of object", "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.type.html", Description = "Select the type of object (reload the page after selecting to populate dropdowns)")]
    public string TypeAlias { get; set; }

    [ConfigurationField("nodeIdSelected", "Property for NodeId", "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.property.html", Description = "Property that holds the nodeid you wish to lookup through")]
    public string NodeIdSelected { get; set; }

    [ConfigurationField("sortColumn", "Column to sort on", "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.column.html", Description = "Enter the column name to sort on")]
    public string SortColumn { get; set; }

    [ConfigurationField("sortOrder", "Sort order", "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/picker.tablesortorder.html", Description = "Select the sortorder")]
    public string SortOrder { get; set; }

    [ConfigurationField("hideCreate", "Hide Create", "boolean")]
    public bool HideCreate { get; set; }

    [ConfigurationField("hideEdit", "Hide Edit", "boolean")]
    public bool HideEdit { get; set; }

    [ConfigurationField("hideSearch", "Hide Search", "boolean")]
    public bool HideSearch { get; set; }

    [ConfigurationField("filters", "Filters", "multivalues", Description = "Here you can set some predefined filters for this list view in this format key|value")]
    public string Filters { get; set; }

    [ConfigurationField("numberOfItems", "Number of items per page", "number", Description = "Type the number of items used per page")]
    public int NumberOfItems { get; set; }
}