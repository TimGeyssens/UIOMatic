using Microsoft.AspNetCore.Mvc;
using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using UIOMatic.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;

namespace UIOMatic.Front.Umbraco.Web.Controllers
{
    [PluginController("UIOMatic")]
    public class PropertyEditorsApiController : UmbracoAuthorizedJsonController
    {
        private readonly IUIOMaticObjectService _service;
        private readonly IUIOMaticHelper _helper;

        public PropertyEditorsApiController(IUIOMaticHelper helper, IUIOMaticObjectService uioMaticObjectService)
        {
            _helper = helper;
            _service = uioMaticObjectService;
        }

        [HttpGet]
        public IActionResult GetPropertyEditors(string typeAlias)
        {
            var type = _helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            var result = _service.GetPropertyEditors(type);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAllTypes()
        {
            var types = _helper.GetUIOMaticTypes();
            return Ok(types);
        }
    }
}