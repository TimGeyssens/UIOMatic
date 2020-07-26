using Umbraco.Core.Composing;

namespace UIOMatic.ContentApps
{
    public class ContentAppsComposer : ICoreComposer
    {
        public void Compose(Composition composition)
        {
            composition.UiomaticContentApps()
                .Append<UiomaticEditorContentAppFactory>();
        }
    }
}
