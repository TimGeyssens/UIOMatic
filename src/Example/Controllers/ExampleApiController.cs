using System.Collections.Generic;
using Umbraco.Core.Persistence;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace Example.Controllers
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