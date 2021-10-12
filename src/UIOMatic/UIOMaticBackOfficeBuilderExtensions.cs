using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UIOMatic.ContentApps;
using UIOMatic.Interfaces;
using UIOMatic.Services;
using UIOMatic.Web;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace UIOMatic
{


    public static class UIOMaticBackOfficeBuilderExtensions
    {
        public static IUmbracoBuilder AddUIOMatic(this IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ServerVariablesParsingNotification, UIOMaticServerVariablesHandler>();

            builder.Services.AddSingleton<IUIOMaticHelper, UIOMaticHelper>();

            //builder.Services.AddTransient<UIOMaticObjectService>();
            

            builder.Services.AddSingleton<IUIOMaticConfiguration, UIOMaticConfiguration>
                                   ((factory) =>
                                   builder.Config
                                          .GetSection("UIOMatic")
                                          .Get<UIOMaticConfiguration>());

            //builder.ContentApps().Append<UiomaticEditorContentAppFactory>();
            builder.UiomaticContentApps().Append<UiomaticEditorContentAppFactory>();


            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<IServiceProviderProxy, HttpContextServiceProviderProxy>();



            return builder;
        }
    }
}
