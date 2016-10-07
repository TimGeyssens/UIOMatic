using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<UIOMaticTypeInfo> GetAllTypes()
        {
            var types = Helper.GetUIOMaticTypes();
            return types.Select(x => _service.GetTypeInfo(x));
        }

        public IEnumerable<object> GetAllObjects(string typeAlias, string sortColumn, string sortOrder)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetAll(t, sortColumn, sortOrder);

        }

        public UIOMaticTypeInfo GetTypeInfo(string typeAlias, bool includePropertyInfo)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetTypeInfo(t, includePropertyInfo);
        }

        public IEnumerable<string> GetAllColumns(string typeAlias)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetAllColumns(t);
        }
    }
}