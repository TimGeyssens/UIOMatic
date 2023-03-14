using UIOMatic.Front.Umbraco.ContentApps;
using UIOMatic.Front.Umbraco.Models.Mapping;
using UIOMatic.Front.Umbraco.Services;
using UIOMatic.Front.Umbraco.Startup.Dashboards;
using UIOMatic.Front.Umbraco.Web;
using UIOMatic.Interfaces;
using UIOMatic.Services;
using UIOMatic.Web;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Notifications;

namespace UIOMatic.Front.Umbraco.Startup
{
    public class UIOMaticComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ServerVariablesParsingNotification, UIOMaticServerVariablesHandler>();

            builder.Services.AddSingleton<IUIOMaticHelper, UIOMaticHelper>();
            builder.Services.AddSingleton<UIOMaticObjectService>();
            builder.Services.AddSingleton<IUIOMaticObjectService, NPocoObjectService>();

            builder.Services.Configure<UIOMaticConfiguration>(builder.Config.GetSection("UIOMatic"));

            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>()
                .Add<UIOMaticTypeInfoMapping>();

            builder.UiomaticContentApps().Append<UiomaticEditorContentAppFactory>();

            builder.AddDashboard<UIOMaticSummaryDashboard>();
        }
    }
}
