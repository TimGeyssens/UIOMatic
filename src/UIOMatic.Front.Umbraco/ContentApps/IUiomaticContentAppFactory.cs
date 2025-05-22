using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;

namespace UIOMatic.Front.Umbraco.ContentApps
{
    /// <summary>
    /// Factory interface for creating UIOMatic content apps
    /// </summary>
    public interface IUiomaticContentAppFactory
    {
        /// <summary>
        /// Gets a content app for the specified type and user groups
        /// </summary>
        /// <param name="type">The type to create the content app for</param>
        /// <param name="userGroups">The user groups that have access to the content app</param>
        /// <returns>A content app instance or null if no app should be created</returns>
        ContentApp GetContentAppFor(Type type, IEnumerable<IReadOnlyUserGroup> userGroups);
    }
}
