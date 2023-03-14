using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Extensions;
using ILogger = Serilog.ILogger;

namespace UIOMatic.Front.Umbraco.ContentApps
{
    public class UiomaticContentAppsFactoryCollectionsBuilder : OrderedCollectionBuilderBase<UiomaticContentAppsFactoryCollectionsBuilder, UiomaticContentAppFactoryCollection, IUiomaticContentAppFactory>
    {
        protected override UiomaticContentAppsFactoryCollectionsBuilder This => this;
    }


    public class UiomaticContentAppFactoryCollection : BuilderCollectionBase<IUiomaticContentAppFactory>
    {
        private readonly ILogger _logger;

        public UiomaticContentAppFactoryCollection(Func<IEnumerable<IUiomaticContentAppFactory>> items, ILogger logger)
            : base(items)
        {
            _logger = logger;
        }


        public IEnumerable<ContentApp> GetContentAppsFor(Type type, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            var apps = this.Select(x => x.GetContentAppFor(type, userGroups)).WhereNotNull().OrderBy(x => x.Weight).ToList();

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
                _logger.Information($"Duplicate content app aliases found: {string.Join(",", dups)}");
            }

            return apps;
        }
    }
    public static class WebCompositionExtensions
    {
        public static UiomaticContentAppsFactoryCollectionsBuilder UiomaticContentApps(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<UiomaticContentAppsFactoryCollectionsBuilder>();
    }
}
