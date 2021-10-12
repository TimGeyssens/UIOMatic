using System.Collections.Generic;
using System.Linq;
using UIOMatic.Services;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace UIOMatic.Web.Controllers
{
    [PluginController("UIOMatic")]
    public class PropertyEditorsApiController: UmbracoAuthorizedJsonController
    {

        private IHttpContextAccessor httpContextAccessor;
        public IHttpContextAccessor HttpContextAccessor => httpContextAccessor ??= HttpContext.RequestServices.GetService<IHttpContextAccessor>();
        
        private IUIOMaticHelper helper;
        public IUIOMaticHelper Helper => helper ??= HttpContext.RequestServices.GetService<IUIOMaticHelper>();



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

        public IEnumerable<string> GetAllColumns(string typeAlias)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetAllColumns(t);
        }
    }
}