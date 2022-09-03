using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIOMatic.Web.Controllers;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Routing;
using Umbraco.Extensions;

namespace UIOMatic.Web
{
    public class UIOMaticServerVariablesHandler : INotificationHandler<ServerVariablesParsingNotification>
    {

        private LinkGenerator linkGenerator;
        private UriUtility uriUtility;

        private readonly IUIOMaticConfiguration Config;

        private string umbracoMvcArea;

        public UIOMaticServerVariablesHandler(LinkGenerator linkGenerator, UriUtility uriUtility,
            IOptions<GlobalSettings> globalSettings,
            Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
            IUIOMaticConfiguration config)
        {
            this.linkGenerator = linkGenerator;
            this.uriUtility = uriUtility;
            Config = config;

             umbracoMvcArea = globalSettings.Value.GetUmbracoMvcArea(hostingEnvironment);

        }

        public void Handle(ServerVariablesParsingNotification notification)
        {
            var mainDictionary = new Dictionary<string, object>
            {
                {
                    "ocBaseUrl",
                    linkGenerator.GetUmbracoApiServiceBaseUrl<ObjectController>(controller => controller.Create(null))
                },
                {
                    "pecBaseUrl",
                    linkGenerator.GetUmbracoApiServiceBaseUrl<PropertyEditorsApiController>(controller =>
                        controller.GetAllTypes())
                },
                {
                    "fcBaseUrl",
                    linkGenerator.GetUmbracoApiServiceBaseUrl<FieldApiController>(
                        controller => controller.GetAllUsers())
                }
            };

            var settingDictionary = new Dictionary<string, object>();

            foreach (var setting in Config?.Settings ?? new Dictionary<string, string>())
                settingDictionary.Add(setting.Key, setting.Value);

            mainDictionary.Add("settings", settingDictionary);

            notification.ServerVariables.TryAdd("uioMatic", mainDictionary);
        }       
    }
}
