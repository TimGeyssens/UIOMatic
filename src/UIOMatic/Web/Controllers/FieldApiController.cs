using System.Collections.Generic;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace UIOMatic.Web.Controllers
{
    [PluginController("UIOMatic")]
    public class FieldApiController : UmbracoAuthorizedJsonController
    {

        private readonly IUserService _userService;
        
        public FieldApiController(IUserService userService)
        {
            _userService = userService;
        }

        public IEnumerable<IUser> GetAllUsers()
        {
            long total = 0;
            return _userService.GetAll(0, 1000, out total); //TODO: Limit what data gets sent down the line
        }
    }
}
