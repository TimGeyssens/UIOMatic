using System.Collections.Generic;
using System.Linq;
using UIOMatic.Services;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace UIOMatic.Front.Umbraco.Web.Controllers
{
    [PluginController("UIOMatic")]
    public class PropertyEditorsApiController: UmbracoAuthorizedJsonController
    {
        private readonly IUIOMaticHelper _helper;
        private readonly IUIOMaticObjectService _service;

        public PropertyEditorsApiController(IUIOMaticObjectService uioMaticObjectService,
            IUIOMaticHelper helper)
        {
            _service = uioMaticObjectService;
            _helper = helper;
        }

        public IEnumerable<UIOMaticTypeInfo> GetAllTypes()
        {
            var types = _helper.GetUIOMaticTypes();
            return types.Select(x => _service.GetTypeInfo(x));
        }

        public IEnumerable<string> GetAllColumns(string typeAlias)
        {
            var t = _helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetAllColumns(t);
        }
    }
}