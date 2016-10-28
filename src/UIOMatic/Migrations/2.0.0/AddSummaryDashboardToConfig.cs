using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace UIOMatic.Migrations._2._0._0
{
    [Migration("2.0.0", 1, Constants.ApplicationAlias)]
    public class AddSummaryDashboardToConfig : MigrationBase
    {
        private addDashboardSection _packageAction;
        private XmlNode _xml;

        public AddSummaryDashboardToConfig(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
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

        public override void Up()
        {
            _packageAction.Execute("uiomatic", _xml);
        }

        public override void Down()
        {
            _packageAction.Undo("uiomatic", _xml);
        }
    }
}
