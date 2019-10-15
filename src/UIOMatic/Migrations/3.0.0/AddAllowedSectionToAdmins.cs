using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Services;
using Umbraco.Web;
using Constants = UIOMatic.Constants;

namespace UIOMatic.Migrations
{
   
    public class AddAllowedSectionToAdmins : MigrationBase
    {
        private readonly IUserService _uService;

        public AddAllowedSectionToAdmins(IMigrationContext context, IUserService uService)
            : base(context)
        {
            _uService = uService;
        }

        public override void Migrate()
        {

            var users = _uService.GetAll(0, 100, out long i)
                .Where(x => x.Groups.Any(y => y.Alias == "admin"));

            foreach (var user in users.Where(user => user.AllowedSections.Contains(Constants.SectionAlias) == false))
            {
                user.AllowedSections.Append(Constants.SectionAlias);
                _uService.Save(user);
            }
        }

        
    }
}
