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
    public class FieldApiController : UmbracoAuthorizedJsonController
    {
        private readonly IUIOMaticObjectService _service;
        private readonly IUIOMaticHelper _helper;

        public FieldApiController(IUIOMaticHelper helper, IUIOMaticObjectService uioMaticObjectService)
        {
            _helper = helper;
            _service = uioMaticObjectService;
        }

        [HttpGet]
        public IActionResult GetFields(string typeAlias)
        {
            var t = _helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            var result = _service.GetFields(t);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _service.GetAllUsers();
            return Ok(users);
        }
    }
}
