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
    /// <summary>
    /// Builder for UIOMatic content app factory collections
    /// </summary>
    public class UiomaticContentAppsFactoryCollectionsBuilder : OrderedCollectionBuilderBase<UiomaticContentAppsFactoryCollectionsBuilder, UiomaticContentAppFactoryCollection, IUiomaticContentAppFactory>
    {
        protected override UiomaticContentAppsFactoryCollectionsBuilder This => this;
    }

    /// <summary>
    /// Collection of UIOMatic content app factories
    /// </summary>
    public class UiomaticContentAppFactoryCollection : BuilderCollectionBase<IUiomaticContentAppFactory>
    {
        private readonly ILogger _logger;

        public UiomaticContentAppFactoryCollection(Func<IEnumerable<IUiomaticContentAppFactory>> items, ILogger logger)
            : base(items)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets content apps for the specified type and user groups
        /// </summary>
        /// <param name="type">The type to get content apps for</param>
        /// <param name="userGroups">The user groups that have access to the content apps</param>
        /// <returns>A collection of content apps</returns>
        public IEnumerable<ContentApp> GetContentAppsFor(Type type, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (userGroups == null)
                throw new ArgumentNullException(nameof(userGroups));

            var apps = this
                .Select(x => x.GetContentAppFor(type, userGroups))
                .WhereNotNull()
                .OrderBy(x => x.Weight)
                .ToList();

            var aliases = new HashSet<string>();
            var duplicates = new List<string>();

            foreach (var app in apps)
            {
                if (aliases.Contains(app.Alias))
                {
                    duplicates.Add(app.Alias);
                }
                else
                {
                    aliases.Add(app.Alias);
                }
            }

            if (duplicates.Any())
            {
                _logger.Warning(
                    "Duplicate content app aliases found: {Duplicates}. This may cause unexpected behavior.",
                    string.Join(", ", duplicates));
            }

            return apps;
        }
    }

    /// <summary>
    /// Extension methods for UIOMatic content apps
    /// </summary>
    public static class WebCompositionExtensions
    {
        /// <summary>
        /// Gets the UIOMatic content apps factory collection builder
        /// </summary>
        /// <param name="builder">The Umbraco builder</param>
        /// <returns>The UIOMatic content apps factory collection builder</returns>
        public static UiomaticContentAppsFactoryCollectionsBuilder UiomaticContentApps(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<UiomaticContentAppsFactoryCollectionsBuilder>();
    }
}
