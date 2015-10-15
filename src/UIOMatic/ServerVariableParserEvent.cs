using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using UIOMatic.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Web.UI.JavaScript;
using System.Web.Routing;
using System.Web.Mvc;
using Umbraco.Web;
using UIOMatic.Controllers;

namespace UIOMatic
{
    public class ServerVariableParserEvent : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;           
        }

        void ServerVariablesParser_Parsing(object sender, Dictionary<string, object> e)
        {
            if (HttpContext.Current == null) return;
            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

            var mainDictionary = new Dictionary<string, object>();
            mainDictionary.Add("ocBaseUrl", urlHelper.GetUmbracoApiServiceBaseUrl<ObjectController>(controller => controller.PostCreate(null)));
            mainDictionary.Add("pecBaseUrl", urlHelper.GetUmbracoApiServiceBaseUrl<PropertyEditorsApiController>(controller => controller.GetAllTypes()));

            if (!e.Keys.Contains("uioMatic"))
            {
                e.Add("uioMatic", mainDictionary);
            }
        }

        
    }
}