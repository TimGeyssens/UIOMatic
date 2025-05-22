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
using Microsoft.Extensions.DependencyInjection;

namespace UIOMatic.Front.Umbraco.Startup
{
    public class UIOMaticComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services
                .AddSingleton<IUIOMaticHelper, UIOMaticHelper>()
                .AddSingleton<UIOMaticObjectService>()
                .AddSingleton<IUIOMaticObjectService, NPocoObjectService>()
                .AddSingleton<UIOMaticServerVariablesHandler>();

            builder.Services.Configure<UIOMaticConfiguration>(
                builder.Config.GetSection(UIOMaticConfiguration.SectionName));

            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>()
                .Add<UIOMaticTypeInfoMapping>();

            builder.UiomaticContentApps()
                .Append<UiomaticEditorContentAppFactory>();

            builder.AddDashboard<UIOMaticSummaryDashboard>();
        }
    }
}
