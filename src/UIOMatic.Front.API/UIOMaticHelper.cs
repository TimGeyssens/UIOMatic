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
using Microsoft.Extensions.Caching.Memory;
using UIOMatic.Front.API.Data;
using UIOMatic.Front.API.Extensions;
using SqlKata.Execution;

namespace UIOMatic.Front.API
{
    public class UIOMaticHelper : IUIOMaticHelper
    {


        private readonly UIOMaticObjectService _uioMaticObjectService;
        private readonly ILogger<IUIOMaticHelper> _logger;
        private readonly IMemoryCache _memCache;
        private readonly QueryFactory _db;

        public UIOMaticHelper(
            UIOMaticObjectService uioMaticObjectService,
            IMemoryCache memCache,
            ILogger<IUIOMaticHelper> logger,
            QueryFactory db)
        {

            _uioMaticObjectService = uioMaticObjectService;
            _logger = logger;
            _memCache = memCache;
            _db = db;
        }


        public  IUIOMaticRepository GetRepository(UIOMaticAttribute attr, UIOMaticTypeInfo typeInfo)
        {
            if (attr.RepositoryType == null)
                return (IUIOMaticRepository)Activator.CreateInstance(typeof(DefaultUIOMaticRepository), attr, typeInfo, _uioMaticObjectService,_db);

            return typeof(DefaultUIOMaticRepository).IsAssignableFrom(attr.RepositoryType)
                ? (IUIOMaticRepository)Activator.CreateInstance(attr.RepositoryType, attr, typeInfo,  _uioMaticObjectService, _db)
                : (IUIOMaticRepository)Activator.CreateInstance(attr.RepositoryType);
        }

        public  IEnumerable<Type> GetUIOMaticTypes()
        {
            return GetUIOMaticFolderTypes().Where(x => x.HasAttribute<UIOMaticAttribute>());
        }

        public  IEnumerable<Type> GetUIOMaticFolderTypes()
        {
            var cachedItems = GetLocalCacheItem<IEnumerable<Type>>("UIOMaticFolderTypes");

            // First cache request, need to set values
            if (cachedItems == null)
            {
                var UIOMaticTypes = EnsureUIOMaticTypes();
                InsertLocalCacheItem("UIOMaticFolderTypes", UIOMaticTypes);
                cachedItems = UIOMaticTypes;
                _logger.LogDebug(string.Format("UIOMaticFolderTypes added to cache and returned from runtime with {0} items", cachedItems.Count()));

            }
            else
            {
                _logger.LogDebug(string.Format("UIOMaticFolderTypes returned directly from cache with {0} items", cachedItems.Count()));
            }

            return cachedItems;
        }

        private  IEnumerable<Type> EnsureUIOMaticTypes()
        {
            var allTypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    allTypes.AddRange(assembly.GetTypes());
                }
                catch (ReflectionTypeLoadException)
                {
                }
            }

            var typesWithMyAttribute =
                from t in allTypes
                let attributes = t.GetCustomAttributes(typeof(UIOMaticFolderAttribute), true)
                where attributes != null && attributes.Length > 0
                select t; // UIOMaticFolderAttribute is the base type for all UIOMatic entities

            // Ensure unique aliases
            var aliases = new List<string>();

            var assemblesWithfolderAttributes = new List<Tuple<string, UIOMaticFolderAttribute>>();

            foreach (var typeWithMyAttribute in typesWithMyAttribute)
            {
                assemblesWithfolderAttributes.Add(new Tuple<string, UIOMaticFolderAttribute>(typeWithMyAttribute.Assembly.FullName, typeWithMyAttribute.GetCustomAttribute<UIOMaticFolderAttribute>(true)));
            }

            // dedupe the list of assemblies and attributes
            assemblesWithfolderAttributes = assemblesWithfolderAttributes.Distinct().ToList();

            foreach (var typeWithMyAttribute in assemblesWithfolderAttributes)
            {
                var attr = typeWithMyAttribute.Item2;
                if (aliases.Any(x => x == attr.Alias))
                    throw new ApplicationException("Multiple UI-O-Matic model types found with alias '" + attr.Alias + "'. Please ensure all types have a unique alias value.");

                aliases.Add(attr.Alias);
            }

            return typesWithMyAttribute;
        }

        public  Type GetUIOMaticTypeByAlias(string typeAlias, bool includeFolders = false, bool throwNullError = false)
        {
            var t = (includeFolders ? GetUIOMaticFolderTypes() : GetUIOMaticTypes()).FirstOrDefault(x => {
                var attr = x.GetCustomAttribute<UIOMaticFolderAttribute>(true);  // UIOMaticFolderAttribute is the base type for all UIOMatic entities
                return attr == null || string.IsNullOrWhiteSpace(attr.Alias)
                    ? x.Name == typeAlias
                    : attr.Alias == typeAlias;
            });

            if (t == null && throwNullError)
            {
                throw new ApplicationException("No UIOMatic type with alias " + typeAlias + " found");
            }

            return t;
        }

        private T GetLocalCacheItem<T>(string cacheKey)
        {
            var cachedItem = _memCache.Get<T>(cacheKey);
            return cachedItem;
        }

        private void InsertLocalCacheItem<T>(string cacheKey, T cacheItem)
        {
             _memCache.Set(cacheKey, cacheItem);
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
