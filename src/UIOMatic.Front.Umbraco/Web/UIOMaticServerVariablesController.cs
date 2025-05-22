using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace UIOMatic.Front.Umbraco.Web
{
    [PluginController("UIOMatic")]
    public class UIOMaticServerVariablesController : UmbracoAuthorizedApiController
    {
        private readonly IUserService _userService;
        private readonly LinkGenerator _linkGenerator;

        public UIOMaticServerVariablesController(
            IUserService userService,
            LinkGenerator linkGenerator)
        {
            _userService = userService;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public IActionResult GetServerVariables()
        {
            var variables = new
            {
                baseUrl = _linkGenerator.GetPathByAction("Index", "UIOMatic", new { area = "UIOMatic" }),
                userService = _userService
            };

            return Ok(variables);
        }
    }
} 