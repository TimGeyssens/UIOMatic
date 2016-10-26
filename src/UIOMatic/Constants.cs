using System.Collections.Generic;

namespace UIOMatic
{
    public static class Constants
    {
        public const string ApplicationAlias = "uiomatic";
        
        public static class FieldEditors
        {
            public const string CheckBox = "checkbox";
            public const string CheckBoxList = "checkboxlist";
            public const string Date = "date";
            public const string DateTime = "datetime";
            public const string DateTimeOffset = "datetimeoffset";
            public const string Dropdown = "dropdown";
            public const string File = "file";
            public const string Label = "label";
            public const string List = "list";
            public const string Map = "map";
            public const string Number = "number";
            public const string Password = "password";
            public const string PickerContent = "pickers.content";
            public const string PickerMedia = "pickers.media";
            public const string PickerMember = "pickers.member";
            public const string PickerUser = "pickers.user";
            public const string RadioButtonList = "radiobuttonlist";
            public const string Rte = "rte";
            public const string Textarea = "textarea";
            public const string Textfield = "textfield";

            public static readonly IReadOnlyDictionary<string, string> ViewPaths = new Dictionary<string, string>
            {
                { "checkbox", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/checkbox.html"},
                { "checkboxlist", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/checkboxlist.html"},
                { "date", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/date.html"},
                { "datetime", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/datetime.html"},
                { "datetimeoffset", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/datetimeoffset.html"},
                { "dropdown", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/dropdown.html"},
                { "file", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/file.html"},
                { "label", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/label.html"},
                { "list", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/list.html"},
                { "map", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/map.html"},
                { "number", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/number.html"},
                { "password", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/password.html"},
                { "pickers.content", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/pickers.content.html"},
                { "pickers.media", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/pickers.media.html"},
                { "pickers.member", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/pickers.member.html"},
                { "pickers.user", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/pickers.user.html"},
                { "radiobuttonlist", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/radiobuttonlist.html"},
                { "rte", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/rte.html"},
                { "textarea", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/textarea.html"},
                { "textfield", "~/app_plugins/uiomatic/backoffice/views/fieldeditors/textfield.html"}
            };
        }

        public static class FieldViews
        {
            public const string Label = "label";
            public const string Image = "image";

            public static readonly IReadOnlyDictionary<string, string> ViewPaths = new Dictionary<string, string>
            {
                { "label", "~/app_plugins/uiomatic/backoffice/views/fieldviews/label.html"},
                { "image", "~/app_plugins/uiomatic/backoffice/views/fieldviews/image.html"}
            };
        }

        public static class FieldFilters
        {
            public const string Dropdown = "dropdown";

            public static readonly IReadOnlyDictionary<string, string> ViewPaths = new Dictionary<string, string>
            {
                { "dropdown", "~/app_plugins/uiomatic/backoffice/views/fieldfilters/dropdown.html"}
            };
        }
    }
}