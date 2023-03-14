using System.Linq;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using static Umbraco.Cms.Core.Constants;

namespace UIOMatic.Front.Umbraco.Migrations
{
    public class AddAllowedSectionToAdmins : MigrationBase
    {
        private readonly IUserService _uService;

        public AddAllowedSectionToAdmins(IMigrationContext context, IUserService uService)
            : base(context)
        {
            _uService = uService;
        }

        protected override void Migrate()
        {
            var userGroup = _uService.GetUserGroupByAlias(Security.AdminGroupAlias);

            if (userGroup != null && !userGroup.AllowedSections.Contains(Constants.SectionAlias))
            {
                userGroup.AddAllowedSection(Constants.SectionAlias);
                _uService.Save(userGroup);
            }
        }
    }
}
