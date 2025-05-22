using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Extensions;

namespace UIOMatic.Front.Umbraco.Migrations
{
    public class AddAllowedSectionToAdmins : MigrationBase
    {
        private readonly IUserService _userService;

        public AddAllowedSectionToAdmins(IMigrationContext context, IUserService userService)
            : base(context)
        {
            _userService = userService;
        }

        protected override void Migrate()
        {
            var adminGroup = _userService.GetUserGroupByAlias("admin");
            if (adminGroup != null)
            {
                var sections = adminGroup.AllowedSections.ToList();
                if (!sections.Contains("uiomatic"))
                {
                    sections.Add("uiomatic");
                    _userService.Save(adminGroup, sections.Select(x => x.GetHashCode()).ToArray());
                }
            }
        }
    }
}
