using System;
using System.Collections.Generic;
using UIOmatic.Services;
using UIOMatic;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace UIOmatic.Web.Controllers
{
    [PluginController("UIOMatic")]
    public class PropertyEditorsApiController: UmbracoAuthorizedJsonController
    {
        private IUIOMaticObjectService _service;

        public PropertyEditorsApiController()
        {
            _service = UIOMaticObjectService.Instance;
        }

        public IEnumerable<Type> GetAllTypes()
        {
            return Helper.GetUIOMaticTypes();
        }

        public IEnumerable<object> GetAllObjects(string typeAlias, string sortColumn, string sortOrder)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetAll(t, sortColumn, sortOrder);

        }

        public IEnumerable<UIOMaticPropertyInfo> GetAllProperties(string typeAlias)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetAllProperties(t, true);
        }

        public IEnumerable<string> GetAllColumns(string typeAlias)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetAllColumns(t);
        }
    }
}