using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIOMatic.Data;
using UIOMatic.Extensions;
using UIOMatic.Interfaces;
using UIOMatic.Attributes;
using UIOMatic.Models;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace UIOMatic
{
    public class Helper
    {
        public static IUIOMaticRepository GetRepository(UIOMaticAttribute attr, UIOMaticTypeInfo typeInfo)
        {
            return typeof(DefaultUIOMaticRepository).IsAssignableFrom(attr.RepositoryType)
                ? (IUIOMaticRepository)Activator.CreateInstance(attr.RepositoryType, attr, typeInfo)
                : (IUIOMaticRepository)Activator.CreateInstance(attr.RepositoryType);
        }

        public static IEnumerable<Type> GetUIOMaticTypes()
        {
            return GetUIOMaticFolderTypes().Where(x => x.HasAttribute<UIOMaticAttribute>());
        }

        public static IEnumerable<Type> GetUIOMaticFolderTypes()
        {
            var cachedItems = GetLocalCacheItem<IEnumerable<Type>>("UIOMaticFolderTypes");

            // First cache request, need to set values
            if (cachedItems == null)
            {
                var UIOMaticTypes = EnsureUIOMaticTypes();
                InsertLocalCacheItem("UIOMaticFolderTypes", () => UIOMaticTypes);
                cachedItems = UIOMaticTypes;
                LogHelper.Debug<Helper>($"UIOMaticFolderTypes added to cache and returned from runtime with {cachedItems.Count()} items");
            }
            else
            {
                LogHelper.Debug<Helper>($"UIOMaticFolderTypes returned directly from cache with {cachedItems.Count()} items");
            }

            return cachedItems;
        }

        private static IEnumerable<Type> EnsureUIOMaticTypes()
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

        public static Type GetUIOMaticTypeByAlias(string typeAlias, bool includeFolders = false, bool throwNullError = false)
        {
            var t = (includeFolders ? GetUIOMaticFolderTypes() : GetUIOMaticTypes()).FirstOrDefault(x => {
                var attr = x.GetCustomAttribute<UIOMaticFolderAttribute>(true);  // UIOMaticFolderAttribute is the base type for all UIOMatic entities
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

        private static T GetLocalCacheItem<T>(string cacheKey)
        {
            var runtimeCache = ApplicationContext.Current.ApplicationCache.RuntimeCache;
            var cachedItem = runtimeCache.GetCacheItem<T>(cacheKey);
            return cachedItem;
        }

        private static void InsertLocalCacheItem<T>(string cacheKey, Func<T> getCacheItem)
        {
            var runtimeCache = ApplicationContext.Current.ApplicationCache.RuntimeCache;
            runtimeCache.InsertCacheItem<T>(cacheKey, getCacheItem);
        }
    }
}