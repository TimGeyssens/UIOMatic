using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;
using Constants = UIOMatic.Constants;

namespace UIOMatic.Migrations._2._0._0
{
    [Migration("2.0.0", 0, Constants.ApplicationAlias)]
    public class AddAllowedSectionToAdmins : MigrationBase
    {
        public AddAllowedSectionToAdmins(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        { }

        public override void Up()
        {
            int i;
            var users = ApplicationContext.Current.Services.UserService.GetAll(0, 100, out i)
                .Where(x => x.UserType.Alias == "admin");

            foreach (var user in users.Where(user => user.AllowedSections.Contains(Constants.ApplicationAlias) == false))
            {
                user.AddAllowedSection(Constants.ApplicationAlias);
                ApplicationContext.Current.Services.UserService.Save(user);
            }
        }

        public override void Down()
        {
            int i;
            var users = ApplicationContext.Current.Services.UserService.GetAll(0, 100, out i)
                .Where(x => x.UserType.Alias == "admin");

            foreach (var user in users.Where(user => user.AllowedSections.Contains(Constants.ApplicationAlias) == false))
            {
                user.RemoveAllowedSection(Constants.ApplicationAlias);
                ApplicationContext.Current.Services.UserService.Save(user);
            }
        }
    }
}
