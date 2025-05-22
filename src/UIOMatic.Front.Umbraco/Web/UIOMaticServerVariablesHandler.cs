using Microsoft.AspNetCore.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace UIOMatic.Front.Umbraco.Web
{
    public class UIOMaticServerVariablesHandler
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IUserService _userService;

        public UIOMaticServerVariablesHandler(
            LinkGenerator linkGenerator,
            IUserService userService)
        {
            _linkGenerator = linkGenerator;
            _userService = userService;
        }
    }
}
