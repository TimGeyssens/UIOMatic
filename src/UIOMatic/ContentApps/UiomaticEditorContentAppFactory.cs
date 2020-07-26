using System;
using Umbraco.Core.Models.ContentEditing;

namespace UIOMatic.ContentApps
{
    internal class UiomaticEditorContentAppFactory : IUiomaticContentAppFactory
    {
        internal const int Weight = -100;

        public ContentApp GetContentAppFor(Type t)
        {
            return new ContentApp
            {
                Alias = "uiomaticContent",
                Name = "Content",
                Icon = Umbraco.Core.Constants.Icons.Content,
                View = "/App_Plugins/UIOMatic/backoffice/uiomatic/edit2.html",
                Weight = Weight
            };
        }
    }
}
