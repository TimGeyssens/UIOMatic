using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;

namespace UIOMatic.ContentApps
{
    internal class UiomaticEditorContentAppFactory : IUiomaticContentAppFactory
    {
        internal const int Weight = -100;


        public ContentApp GetContentAppFor(Type type, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            return new ContentApp
            {
                Alias = "uiomaticContent",
                Name = "Content",
                Icon = Umbraco.Cms.Core.Constants.Icons.Content,
                View = "/App_Plugins/UIOMatic/backoffice/apps/uiomaticContent.html",
                Weight = Weight
            };
        }
    }
}
