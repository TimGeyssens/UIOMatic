using System.Collections.Generic;
using Umbraco.Core.Models.Membership;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace UIOmatic.Web.Controllers
{
    [PluginController("UIOMatic")]
    public class FieldApiController : UmbracoAuthorizedJsonController
    {
        public IEnumerable<IUser> GetAllUsers()
        {
            var total = 0;
            var us = Services.UserService;
            return us.GetAll(0, 1000, out total);
        }
    }
}