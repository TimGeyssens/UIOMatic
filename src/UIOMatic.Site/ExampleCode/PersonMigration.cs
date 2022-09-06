using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

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

        public PersonMigrationComponent(ICoreScopeProvider coreScopeProvider,
            IMigrationPlanExecutor migrationPlanExecutor,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState)
        {
            _coreScopeProvider = coreScopeProvider;
            _migrationPlanExecutor = migrationPlanExecutor;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
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
            throw new NotImplementedException();
        }
    }

    public class PersonMigration : MigrationBase
    {
        public PersonMigration(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            if (!TableExists("People"))
                Create.Table<Person>().Do();
        }
    }
}
