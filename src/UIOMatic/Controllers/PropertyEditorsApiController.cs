using System;
using System.Collections.Generic;
using UIOMatic.Core;
using UIOMatic.Core.Interfaces;
using UIOMatic.Core.Models;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace UIOMatic.Controllers
{
    [PluginController("UIOMatic")]
    public class PropertyEditorsApiController: UmbracoAuthorizedJsonController
    {
        public IEnumerable<Type> GetAllTypes()
        {
            return Helper.GetTypesWithUIOMaticAttribute();
        }

        public IEnumerable<object> GetAllObjects(string typeName, string sortColumn, string sortOrder)
        {
            object ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetAll(typeName, sortColumn, sortOrder);
        }

        public IEnumerable<UIOMaticPropertyInfo> GetAllProperties(string typeName)
        {
            object ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetAllProperties(typeName,true);
        }

        public IEnumerable<string> GetAllColumns(string typeName)
        {
            object ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetAllColumns(typeName);
        }
    }
}