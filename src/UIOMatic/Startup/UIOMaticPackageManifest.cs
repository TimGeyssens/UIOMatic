using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Core.PropertyEditors;

namespace UIOMatic.Startup;

public class UIOMaticPackageManifest : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        manifests.Add(new PackageManifest
        {
            Scripts = new []
            {
                "/App_Plugins/UIOMatic/backoffice/assets/js/angular-relative-date.js",
                "/App_Plugins/UIOMatic/backoffice/assets/js/moment-with-locales.js",
                "/App_Plugins/UIOMatic/backoffice/assets/js/bootstrap-datetimepicker.js",
                "/App_Plugins/UIOMatic/backoffice/imports.js",
                "/App_Plugins/UIOMatic/backoffice/services/utility.service.js",
                "/App_Plugins/UIOMatic/backoffice/directives/pagination.directive.js",
                "/App_Plugins/UIOMatic/backoffice/resources/uioMaticObject.resource.js",
                "/App_Plugins/UIOMatic/backoffice/resources/uioMaticField.resource.js",
                "/App_Plugins/UIOMatic/backoffice/resources/uioMaticPropertyEditor.resource.js",
                "/App_Plugins/UIOMatic/backoffice/uiomatic/edit.controller.js",
                "/App_Plugins/UIOMatic/backoffice/uiomatic/delete.controller.js",
                "/App_Plugins/UIOMatic/backoffice/uiomatic/list.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/file.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/datetime.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/dropdown.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/pickers.content.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/pickers.media.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/pickers.member.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/pickers.user.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/pickers.users.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/checkboxlist.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/radiobuttonlist.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/label.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/list.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/rte.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/map.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldeditors/link.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/fieldfilters/dropdown.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/dropdown.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/listview.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/listview.dialog.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/multipicker.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.type.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.property.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/propertyeditors/pickers.column.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/dialogs/objectsearcher.controller.js",
                "/App_Plugins/UIOMatic/backoffice/views/dashboards/summarydashboard.controller.js",
                "/App_Plugins/UIOMatic/backoffice/apps/uiomaticContent.controller.js"
            },
            Stylesheets = new []
            {
                "/App_Plugins/UIOMatic/backoffice/assets/css/uiomatic.css",
                "/App_Plugins/UIOMatic/backoffice/assets/css/bootstrap-datetimepicker.min.css"
            }
        });
    }
}