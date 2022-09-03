using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UIOMatic.ContentApps;
using UIOMatic.Interfaces;
using UIOMatic.Services;
using UIOMatic.Web;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace UIOMatic.Startup
{
    public class UIOMaticComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ServerVariablesParsingNotification, UIOMaticServerVariablesHandler>();

            builder.Services.AddSingleton<IUIOMaticHelper, UIOMaticHelper>();
            builder.Services.AddSingleton<UIOMaticObjectService>();
            builder.Services.AddSingleton<IUIOMaticObjectService, NPocoObjectService>();

            builder.Services.AddSingleton<IUIOMaticConfiguration, UIOMaticConfiguration>
            ((factory) =>
                builder.Config
                    .GetSection("UIOMatic")
                    .Get<UIOMaticConfiguration>());
            
            builder.UiomaticContentApps().Append<UiomaticEditorContentAppFactory>();
        }
    }
}
