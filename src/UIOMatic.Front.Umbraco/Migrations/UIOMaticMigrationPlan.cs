using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Packaging;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.Front.Umbraco.Migrations
{
    public class UIOMaticMigrationPlan : PackageMigrationPlan
    {
        private readonly IUserService _userService;

        public UIOMaticMigrationPlan(IUserService userService) 
            : base("UIOMatic", "UIOMatic")
        {
            _userService = userService;
        }

        protected override void DefinePlan()
        {
            To<AddAllowedSectionToAdmins>("state-1");
        }
    }
}
