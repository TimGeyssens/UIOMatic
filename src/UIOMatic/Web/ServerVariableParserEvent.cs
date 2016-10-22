using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UIOMatic.Web.Controllers;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.UI.JavaScript;

namespace UIOMatic.Web
{
    public class ServerVariableParserEvent : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ServerVariablesParser.Parsing += this.ServerVariablesParser_Parsing;           
        }

        void ServerVariablesParser_Parsing(object sender, Dictionary<string, object> e)
        {
            if (HttpContext.Current == null) return;
            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

            var mainDictionary = new Dictionary<string, object>
            {
                {
                    "ocBaseUrl",
                    urlHelper.GetUmbracoApiServiceBaseUrl<ObjectController>(controller => controller.Create(null))
                },
                {
                    "pecBaseUrl",
                    urlHelper.GetUmbracoApiServiceBaseUrl<PropertyEditorsApiController>(
                        controller => controller.GetAllTypes())
                },
                {
                    "fcBaseUrl",
                    urlHelper.GetUmbracoApiServiceBaseUrl<FieldApiController>(controller => controller.GetAllUsers())
                }
            };

            if (!e.Keys.Contains("uioMatic"))
            {
                e.Add("uioMatic", mainDictionary);
            }
        }

        
    }
}