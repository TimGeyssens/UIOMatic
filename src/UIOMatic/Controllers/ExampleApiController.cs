using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UIOMatic.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace UIOMatic.Controllers
{
    [PluginController("Example")]
    public class ExampleApiController: UmbracoAuthorizedJsonController
    {
        public IEnumerable<dynamic> GetAll()
        {
            var query = new Sql().Select("*").From("People");
            return DatabaseContext.Database.Fetch<dynamic>(query);
        }
    }
}