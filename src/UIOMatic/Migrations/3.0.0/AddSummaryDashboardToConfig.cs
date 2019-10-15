using System.Xml;
using Umbraco.Core.PackageActions;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace UIOMatic.Migrations
{
   
    public class AddSummaryDashboardToConfig : MigrationBase
    {
        private addDashboardSection _packageAction;
        private XmlNode _xml;

        public AddSummaryDashboardToConfig(IMigrationContext context)
            : base(context)
        {
            _packageAction = new addDashboardSection();

            var xml = @"<Action runat=""install"" alias=""addDashboardSection"" dashboardAlias=""UIOMaticSummaryDashboard"">
    <section>
        <areas>
            <area>uiomatic</area>
        </areas>
        <tab caption=""Summary"">
            <control>../app_plugins/uiomatic/backoffice/views/dashboards/summarydashboard.html</control>
        </tab>  
    </section>
</Action>";

            var xdoc = new XmlDocument();
            xdoc.LoadXml(xml);

            _xml = xdoc.DocumentElement;
        }

        public override void Migrate()
        {
            _packageAction.Execute("uiomatic", _xml);
        }

        
    }
}
