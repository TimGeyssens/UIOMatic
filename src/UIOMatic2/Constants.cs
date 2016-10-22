using System.Collections.Generic;

namespace UIOMatic
{
    public static class Constants
    {
        public const string ApplicationAlias = "uiomatic";

        // TODO: Add paths to all built in views
        public static readonly IReadOnlyDictionary<string, string> Views = new Dictionary<string, string>
        {
            { "textfield", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/checkbox.html"},
            { "datetime", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/datetime.html"},
            { "number", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/number.html"}
        }; 
    }
}