using System.Collections.Generic;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;

namespace UIOMatic;

internal class ManifestLoader : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.ManifestFilters().Append<ManifestFilter>();
    }
}

internal class ManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        manifests.Add(new PackageManifest
        {
            PackageName = "UI-O-Matic",
            AllowPackageTelemetry = true,
            Scripts = new []
            {
                "/App_Plugins/UIOMatic/backoffice/components/list/uiomatic-list.js",
                "/App_Plugins/UIOMatic/backoffice/components/edit/uiomatic-edit.js"
            },
            Stylesheets = new []
            {
                "/App_Plugins/UIOMatic/backoffice/assets/css/uiomatic.css"
            }
        });
    }
}