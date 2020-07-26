using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.ContentEditing;
using Umbraco.Web.ContentApps;

namespace UIOMatic.ContentApps
{
    public class UiomaticContentAppsFactoryCollectionsBuilder : OrderedCollectionBuilderBase<UiomaticContentAppsFactoryCollectionsBuilder, UiomaticContentAppFactoryCollection, IUiomaticContentAppFactory>
    {
        protected override UiomaticContentAppsFactoryCollectionsBuilder This => this;
    }


    public class UiomaticContentAppFactoryCollection : BuilderCollectionBase<IUiomaticContentAppFactory>
    {
        private readonly ILogger _logger;

        public UiomaticContentAppFactoryCollection(IEnumerable<IUiomaticContentAppFactory> items, ILogger logger)
            : base(items)
        {
            _logger = logger;
        }


        public IEnumerable<ContentApp> GetContentAppsFor(Type type)
        {
            var apps = this.Select(x => x.GetContentAppFor(type)).WhereNotNull().OrderBy(x => x.Weight).ToList();

            var aliases = new HashSet<string>();
            List<string> dups = null;

            foreach (var app in apps)
            {
                if (aliases.Contains(app.Alias))
                    (dups ?? (dups = new List<string>())).Add(app.Alias);
                else
                    aliases.Add(app.Alias);
            }

            if (dups != null)
            {
                // dying is not user-friendly, so let's write to log instead, and wish people read logs...

                //throw new InvalidOperationException($"Duplicate content app aliases found: {string.Join(",", dups)}");
                _logger.Warn<UiomaticContentAppFactoryCollection>("Duplicate content app aliases found: {DuplicateAliases}", string.Join(",", dups));
            }

            return apps;
        }


    }
    public static class WebCompositionExtensions
    {
        public static UiomaticContentAppsFactoryCollectionsBuilder UiomaticContentApps(this Composition composition)
            => composition.WithCollectionBuilder<UiomaticContentAppsFactoryCollectionsBuilder>();
    }


}
