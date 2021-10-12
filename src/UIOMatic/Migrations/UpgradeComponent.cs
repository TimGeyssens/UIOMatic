
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Umbraco.Core.Composing;
//using Umbraco.Core.Migrations;
//using Umbraco.Core.Scoping;
//using Umbraco.Core.Services;
//using Umbraco.Core.Logging;
//using Umbraco.Core.Migrations.Upgrade;
//using Umbraco.Core;

//namespace UIOMatic.Migrations
//{
//    public class UpgradeComponentComposer : IUserComposer
//    {
//        public void Compose(Composition composition)
//        {
//            composition.Components().Append<UpgradeComponent>();
//        }
//    }

//    public class UpgradeComponent : IComponent
//    {
//        private readonly IScopeProvider _scopeProvider;
//        private readonly IMigrationBuilder _migrationBuilder;
//        private readonly IKeyValueService _keyValueService;
//        private readonly ILogger _logger;

//        public UpgradeComponent(IScopeProvider scopeProvider, IMigrationBuilder migrationBuilder, IKeyValueService keyValueService, ILogger logger)
//        {
//            _scopeProvider = scopeProvider;
//            _migrationBuilder = migrationBuilder;
//            _keyValueService = keyValueService;
//            _logger = logger;
//        }
//        public void Initialize()
//        {




//            var pingplan = new MigrationPlan("UIOMaticPing");
//            pingplan.From(string.Empty)
//                .To<InstancePing>("state-1");

//            var upgraderping = new Upgrader(pingplan);
//            upgraderping.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);

//            try
//            {
//                var plan = new MigrationPlan("UIOMatic");
//                plan.From(string.Empty)
//                    .To<AddAllowedSectionToAdmins>("state-3.0.0");

//                var upgrader = new Upgrader(plan);
//                upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);
//            }
//            catch { }

           


//        }

//        public void Terminate()
//        {
//        }
//    }
//}
