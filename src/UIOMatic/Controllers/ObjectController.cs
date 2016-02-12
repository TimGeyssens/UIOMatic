using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace UIOMatic.Controllers
{
    [PluginController("UIOMatic")]
    public class QueryController : UmbracoApiController
    {
        public UIOMaticPagedResult PostQuery(UIOMaticQueryInfo queryinfo)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetQuery(queryinfo);
        }
    }
    [PluginController("UIOMatic")]
    public class ObjectController : UmbracoAuthorizedJsonController
    {
        public IEnumerable<object> GetAll(string typeName, string sortColumn, string sortOrder)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetAll(typeName, sortColumn, sortOrder);
        }

        public IEnumerable<object> GetFiltered(string typeName, string filterColumn, string filterValue, string sortColumn, string sortOrder)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetFiltered(typeName,filterColumn,filterValue, sortColumn, sortOrder);
        }

        public UIOMaticPagedResult PostQuery(UIOMaticQueryInfo queryinfo)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetQuery(queryinfo);
            //return null;
        }

        public IEnumerable<object> GetFiltered(string typeName, string filterColumn, string filterValue, string sortColumn, string sortOrder)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetFiltered(typeName,filterColumn,filterValue, sortColumn, sortOrder);
        }

        public UIOMaticPagedResult GetPaged(string typeName, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, string searchTerm)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetPaged(typeName, itemsPerPage, pageNumber, sortColumn, sortOrder, searchTerm);
        }

        public IEnumerable<Models.UIOMaticPropertyInfo> GetAllProperties(string typeName, bool isEdit)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetAllProperties(typeName, isEdit);
        }

        public IEnumerable<Models.UIOMaticFilterPropertyInfo> GetFilterProperties(string typeName)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetFilterProperties(typeName);
        }

        public UIOMaticTypeInfo GetType(string typeName)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetType(typeName);

        }

        public object GetById(string typeName, string id)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetById(typeName, id);
        }

        public object PostCreate(System.Dynamic.ExpandoObject objectToCreate)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).PostCreate(objectToCreate);
        }

        public object PostUpdate(System.Dynamic.ExpandoObject objectToUpdate)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).PostUpdate(objectToUpdate);
        }

        public string[] DeleteByIds(string typeOfObject, string ids)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).DeleteByIds(typeOfObject, ids);
        }

        public IEnumerable<Exception> Validate(System.Dynamic.ExpandoObject objectToValidate)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).Validate(objectToValidate);
        }

        public UIOMaticPagedResult PostQuery(UIOMaticQueryInfo queryinfo)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetQuery(queryinfo);
            //return null;
        }

        public IEnumerable<Models.UIOMaticFilterPropertyInfo> GetFilterProperties(string typeName)
        {
            var ctrl = Activator.CreateInstance(Config.DefaultObjectControllerType, null);
            return ((IUIOMaticObjectController)ctrl).GetFilterProperties(typeName);
        }
    }

}