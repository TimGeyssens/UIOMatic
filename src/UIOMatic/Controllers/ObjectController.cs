using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace UIOMatic.Controllers
{
    [PluginController("UIOMatic")]
    public class ObjectController : UmbracoAuthorizedJsonController
    {
        public IEnumerable<object> GetAll(string typeName, string sortColumn, string sortOrder)
        {
            object ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController) ctrl).GetAll(typeName, sortColumn, sortOrder);
        }

        public IEnumerable<Models.UIOMaticPropertyInfo> GetAllProperties(string typeName)
        {
            object ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController) ctrl).GetAllProperties(typeName);
        }

        public UIOMaticTypeInfo GetType(string typeName)
        {
            object ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetType(typeName);

        }

        public object GetById(string typeName, int id)
        {
            object ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetById(typeName, id);
        }

        public object PostCreate(System.Dynamic.ExpandoObject objectToCreate)
        {
            object ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).PostCreate(objectToCreate);
        }

        public object PostUpdate(System.Dynamic.ExpandoObject objectToUpdate)
        {
            object ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).PostUpdate(objectToUpdate);
        }

        public int[] DeleteByIds(string typeOfObject, string ids)
        {
            object ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).DeleteByIds(typeOfObject,ids);
        }

        public IEnumerable<Exception> Validate(System.Dynamic.ExpandoObject objectToValidate)
        {
            object ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).Validate(objectToValidate);
        }
    }
}