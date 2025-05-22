using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using UIOMatic.Data;
using UIOMatic.Extensions;
using UIOMatic.Interfaces;
using UIOMatic.Attributes;
using UIOMatic.Models;
using UIOMatic.Services;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Scoping;
using UIOMatic.Front.Umbraco.Data;
using UIOMatic.Front.Umbraco.Extensions;
using IHostingEnvironment = Umbraco.Cms.Core.Hosting.IHostingEnvironment;

namespace UIOMatic.Front.Umbraco
{
    public class UIOMaticHelper : IUIOMaticHelper
    {
        private readonly IAppPolicyCache _runtimeCache;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ICoreScopeProvider _scopeProvider;
        private readonly UIOMaticObjectService _uioMaticObjectService;
        private readonly ILogger<IUIOMaticHelper> _logger;

        public UIOMaticHelper(
            IAppPolicyCache runtimeCache,
            IHostingEnvironment hostingEnvironment,
            ICoreScopeProvider scopeProvider,
            UIOMaticObjectService uioMaticObjectService,
            ILogger<IUIOMaticHelper> logger)
        {
            _runtimeCache = runtimeCache;
            _hostingEnvironment = hostingEnvironment;
            _scopeProvider = scopeProvider;
            _uioMaticObjectService = uioMaticObjectService;
            _logger = logger;
        }

        public IUIOMaticRepository GetRepository(UIOMaticAttribute attr, UIOMaticTypeInfo typeInfo)
        {
            if (attr.RepositoryType == null)
                return (IUIOMaticRepository)Activator.CreateInstance(typeof(DefaultUIOMaticRepository), attr, typeInfo, _scopeProvider, _uioMaticObjectService);

            return typeof(DefaultUIOMaticRepository).IsAssignableFrom(attr.RepositoryType)
                ? (IUIOMaticRepository)Activator.CreateInstance(attr.RepositoryType, attr, typeInfo, _scopeProvider, _uioMaticObjectService)
                : (IUIOMaticRepository)Activator.CreateInstance(attr.RepositoryType, _scopeProvider);
        }

        public IEnumerable<Type> GetUIOMaticTypes()
        {
            return GetUIOMaticFolderTypes().Where(x => x.HasAttribute<UIOMaticAttribute>());
        }

        public IEnumerable<Type> GetUIOMaticFolderTypes()
        {
            return _runtimeCache.GetCacheItem("UIOMaticFolderTypes", () => EnsureUIOMaticTypes());
        }

        private IEnumerable<Type> EnsureUIOMaticTypes()
        {
            var types = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    types.AddRange(assembly.GetTypes().Where(x => x.HasAttribute<UIOMaticFolderAttribute>()));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading types from assembly {Assembly}", assembly.FullName);
                }
            }
            return types;
        }

        public Type GetUIOMaticTypeByAlias(string typeAlias, bool includeFolders = false, bool throwNullError = false)
        {
            var t = (includeFolders ? GetUIOMaticFolderTypes() : GetUIOMaticTypes()).FirstOrDefault(x => {
                var attr = x.GetCustomAttribute<UIOMaticFolderAttribute>(true);
                return attr == null || attr.Alias.IsNullOrWhiteSpace()
                    ? x.Name == typeAlias
                    : attr.Alias == typeAlias;
            });

            if (t == null && throwNullError)
            {
                throw new ApplicationException("No UIOMatic type with alias " + typeAlias + " found");
            }

            return t;
        }
    }

    public interface IUIOMaticHelper
    {
        IUIOMaticRepository GetRepository(UIOMaticAttribute attr, UIOMaticTypeInfo typeInfo);
        IEnumerable<Type> GetUIOMaticTypes();
        IEnumerable<Type> GetUIOMaticFolderTypes();
        Type GetUIOMaticTypeByAlias(string typeAlias, bool includeFolders = false, bool throwNullError = false);
    }
}
