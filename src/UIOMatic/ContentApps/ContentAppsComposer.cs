using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace UIOMatic.ContentApps
{
    public class ContentAppsComposer : IComposer
    {

        public void Compose(IUmbracoBuilder builder)
        {
            builder.ContentApps().Append<UiomaticEditorContentAppFactory>();
        }
    }
}
