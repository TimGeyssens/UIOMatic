using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using UIOMatic.Core.Migrations;
using UIOMatic.Interfaces;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseModelDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIOMatic.Front.Umbraco.Migrations;

namespace UIOMatic.Site.ExampleCode
{
    public class PersonMigrationComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddComponent<PersonMigrationComponent>();
        }
    }

    public class PersonMigrationComponent : IComponent
    {
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;
        private readonly IUIOMaticHelper _helper;

        public PersonMigrationComponent(
            ICoreScopeProvider coreScopeProvider,
            IMigrationPlanExecutor migrationPlanExecutor,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState,
            IUIOMaticHelper helper)
        {
            _coreScopeProvider = coreScopeProvider;
            _migrationPlanExecutor = migrationPlanExecutor;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
            _helper = helper;
        }

        public void Initialize()
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }
            
            var migrationPlan = new MigrationPlan("TestUIOMatic");
            
            migrationPlan.From(string.Empty)
                .To<PersonMigration>("state-1");
            
            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
        }

        public void Terminate()
        {
        }
    }

    public class PersonMigration : UIOMaticMigrationBase
    {
        public override void Migrate()
        {
            EnsureTableExists<Person>();
            EnsureColumnExists<Person>(x => x.FirstName);
            EnsureColumnExists<Person>(x => x.LastName);
            EnsureColumnExists<Person>(x => x.Email);
            EnsureColumnExists<Person>(x => x.Phone);
            EnsureColumnExists<Person>(x => x.Address);
            EnsureColumnExists<Person>(x => x.City);
            EnsureColumnExists<Person>(x => x.Country);
            EnsureColumnExists<Person>(x => x.PostalCode);
            EnsureColumnExists<Person>(x => x.Created);
            EnsureColumnExists<Person>(x => x.Updated);
            EnsureIndexExists<Person>(x => x.Email, true);
        }
    }
}
