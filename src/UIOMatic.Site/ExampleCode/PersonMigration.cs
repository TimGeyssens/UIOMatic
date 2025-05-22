using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using UIOMatic.Front.Umbraco.Migrations;
using UIOMatic.Front.Umbraco;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseModelDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

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
        public PersonMigration(IMigrationContext context, IUIOMaticHelper helper) 
            : base(context, helper)
        {
        }

        protected override void Migrate()
        {
            Create.Table("Person");
            Alter.Table("Person")
                .AddColumn("FirstName").AsString()
                .AddColumn("LastName").AsString()
                .AddColumn("Email").AsString()
                .AddColumn("Phone").AsString()
                .AddColumn("Address").AsString()
                .AddColumn("City").AsString()
                .AddColumn("Country").AsString()
                .AddColumn("PostalCode").AsString()
                .AddColumn("Created").AsDateTime()
                .AddColumn("Updated").AsDateTime();

            Create.Index("IX_Person_Email")
                .OnTable("Person")
                .OnColumn("Email")
                .Ascending()
                .WithOptions().Unique();
        }
    }
}
