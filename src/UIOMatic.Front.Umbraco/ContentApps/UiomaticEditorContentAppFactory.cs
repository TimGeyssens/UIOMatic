using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;
using static Umbraco.Cms.Core.Constants;

namespace UIOMatic.Front.Umbraco.ContentApps
{
    /// <summary>
    /// Factory for creating UIOMatic editor content apps
    /// </summary>
    internal class UiomaticEditorContentAppFactory : IUiomaticContentAppFactory
    {
        /// <summary>
        /// The weight of the content app
        /// </summary>
        internal const int Weight = -100;

        /// <inheritdoc />
        public ContentApp GetContentAppFor(Type type, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            return new ContentApp
            {
                Alias = "uiomaticContent",
                Name = "Content",
                Icon = Icons.Content,
                View = "/App_Plugins/UIOMatic/backoffice/apps/uiomaticContent.html",
                Weight = Weight
            };
        }
    }
}
