using Umbraco.Cms.Core.PropertyEditors;

namespace UIOMatic.Front.Umbraco.DataEditors.ListView;

/// <summary>
/// Configuration for the UIOMatic list view editor
/// </summary>
public class UIOMaticListViewConfiguration
{
    /// <summary>
    /// Gets or sets the type alias of the object to display in the list view
    /// </summary>
    [ConfigurationField(
        "typeAlias",
        "Type of object",
        "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.type.html",
        Description = "Select the type of object (reload the page after selecting to populate dropdowns)")]
    public string TypeAlias { get; set; }

    /// <summary>
    /// Gets or sets the property that holds the node ID to lookup through
    /// </summary>
    [ConfigurationField(
        "nodeIdSelected",
        "Property for NodeId",
        "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.property.html",
        Description = "Property that holds the nodeid you wish to lookup through")]
    public string NodeIdSelected { get; set; }

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

    /// <summary>
    /// Gets or sets whether to hide the create button
    /// </summary>
    [ConfigurationField(
        "hideCreate",
        "Hide Create",
        "boolean")]
    public bool HideCreate { get; set; }

    /// <summary>
    /// Gets or sets whether to hide the edit button
    /// </summary>
    [ConfigurationField(
        "hideEdit",
        "Hide Edit",
        "boolean")]
    public bool HideEdit { get; set; }

    /// <summary>
    /// Gets or sets whether to hide the search functionality
    /// </summary>
    [ConfigurationField(
        "hideSearch",
        "Hide Search",
        "boolean")]
    public bool HideSearch { get; set; }

    /// <summary>
    /// Gets or sets the predefined filters for the list view
    /// </summary>
    [ConfigurationField(
        "filters",
        "Filters",
        "multivalues",
        Description = "Here you can set some predefined filters for this list view in this format key|value")]
    public string Filters { get; set; }

    /// <summary>
    /// Gets or sets the number of items to display per page
    /// </summary>
    [ConfigurationField(
        "numberOfItems",
        "Number of items per page",
        "number",
        Description = "Type the number of items used per page")]
    public int NumberOfItems { get; set; }
}