using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Packaging;

namespace UIOMatic.Migrations
{
    public class UIOMaticMigrationPlan : PackageMigrationPlan
    {
        public UIOMaticMigrationPlan() : base("UIOMatic", "UIOMatic")
        {
        }

        protected override void DefinePlan()
        {
            To<AddAllowedSectionToAdmins>("state-3.0.0");
        }
    }
}
