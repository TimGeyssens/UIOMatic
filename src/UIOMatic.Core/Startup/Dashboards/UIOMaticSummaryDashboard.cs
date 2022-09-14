using System;
using Umbraco.Cms.Core.Dashboards;

namespace UIOMatic.Startup.Dashboards;

public class UIOMaticSummaryDashboard : IDashboard
{
    public string Alias => "UIOMaticSummaryDashboard";
    public string View => "/app_plugins/uiomatic/backoffice/views/dashboards/summarydashboard.html";
    public string[] Sections => new[] { "uiomatic" };
    public IAccessRule[] AccessRules => Array.Empty<IAccessRule>();
}