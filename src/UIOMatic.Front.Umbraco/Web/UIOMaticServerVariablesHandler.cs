using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using UIOMatic.Front.Umbraco.Web.Controllers;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace UIOMatic.Front.Umbraco.Web
{
    public class UIOMaticServerVariablesHandler : INotificationHandler<ServerVariablesParsingNotification>
    {
        private LinkGenerator _linkGenerator;
        private UIOMaticConfiguration _config;

        public UIOMaticServerVariablesHandler(LinkGenerator linkGenerator,
            IOptionsMonitor<UIOMaticConfiguration> configOptions)
        {
            _linkGenerator = linkGenerator;

            _config = configOptions.CurrentValue;
            configOptions.OnChange(newValue => _config = newValue);
        }

        public void Handle(ServerVariablesParsingNotification notification)
        {
            var mainDictionary = new Dictionary<string, object>
            {
                {
                    "ocBaseUrl",
                    _linkGenerator.GetUmbracoApiServiceBaseUrl<ObjectController>(controller => controller.Create(null))
                },
                {
                    "pecBaseUrl",
                    _linkGenerator.GetUmbracoApiServiceBaseUrl<PropertyEditorsApiController>(controller =>
                        controller.GetAllTypes())
                },
                {
                    "fcBaseUrl",
                    _linkGenerator.GetUmbracoApiServiceBaseUrl<FieldApiController>(
                        controller => controller.GetAllUsers())
                }
            };

            var settingDictionary = new Dictionary<string, object>();

            settingDictionary.Add("defaultListViewPageSize", _config.DefaultListViewPageSize);
            settingDictionary.Add("rteFieldEditorButtons", _config.RteFieldEditorButtons);

            mainDictionary.Add("settings", settingDictionary);

            notification.ServerVariables.TryAdd("uioMatic", mainDictionary);
        }
    }
}
