using System;
using UIOMatic.Data;
using UIOMatic.Enums;
using UIOMatic.Interfaces;

namespace UIOMatic.Attributes
{
    /// <summary>
    /// Attribute to mark a class as a UIOMatic type, defining how it should be displayed and managed in the UI
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UIOMaticAttribute : UIOMaticFolderAttribute
    {
        /// <summary>
        /// Gets or sets the name of individual items of this type
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the icon to use for individual items
        /// </summary>
        public string ItemIcon { get; set; }

        /// <summary>
        /// Gets or sets how this type should be rendered in the UI (Tree or List view)
        /// </summary>
        public UIOMaticRenderType RenderType { get; set; }

        /// <summary>
        /// Gets or sets the repository type to use for this type
        /// </summary>
        public Type RepositoryType { get; set; }

        /// <summary>
        /// Gets or sets the name of the connection string to use
        /// </summary>
        public string ConnectionStringName { get; set; }

        /// <summary>
        /// Gets or sets the default column to sort by
        /// </summary>
        public string SortColumn { get; set; }

        /// <summary>
        /// Gets or sets the default sort order (ASC or DESC)
        /// </summary>
        public string SortOrder { get; set; }

        /// <summary>
        /// Gets or sets whether this type is read-only
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets whether this type should be hidden from the tree
        /// </summary>
        public bool HideFromTree { get; set; }

        /// <summary>
        /// Gets or sets the list view actions available for this type
        /// </summary>
        public Type[] ListViewActions { get; set; }

        /// <summary>
        /// Gets or sets whether this type should be shown on the summary dashboard
        /// </summary>
        public bool ShowOnSummaryDashboard { get; set; }

        /// <summary>
        /// Gets or sets the name of the column used to track soft deletes
        /// </summary>
        public string DeletedColumnName { get; set; }

        /// <summary>
        /// Gets or sets any additional parameters to pass to the repository constructor
        /// </summary>
        public object[] RepositoryParameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the UIOMaticAttribute class
        /// </summary>
        /// <param name="alias">The unique alias for this type</param>
        /// <param name="folderName">The name of the folder this type belongs to</param>
        /// <param name="itemName">The name of individual items of this type</param>
        public UIOMaticAttribute(string alias, string folderName, string itemName)
            : base(alias, folderName)
        {
            ItemName = itemName;
            ItemIcon = "icon-umb-content";
            RenderType = UIOMaticRenderType.Tree;
            SortOrder = "ASC";
            ReadOnly = false;
            //RepositoryType = typeof(DefaultUIOMaticRepository);
            HideFromTree = false;
            ListViewActions = null;
            ShowOnSummaryDashboard = false;
        }
    }
}