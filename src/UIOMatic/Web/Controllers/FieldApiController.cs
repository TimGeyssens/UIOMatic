using System.Collections.Generic;
using Umbraco.Core.Models.Membership;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace UIOMatic.Web.Controllers
{
    [PluginController("UIOMatic")]
    public class FieldApiController : UmbracoAuthorizedJsonController
    {
        public IEnumerable<IUser> GetAllUsers()
        {
            long total = 0;
            var us = Services.UserService;
            return us.GetAll(0, 1000, out total); //TODO: Limit what data gets sent down the line
        }
    }
}
