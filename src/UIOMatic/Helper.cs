using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UIOMatic.Data;
using UIOMatic.Extensions;
using UIOMatic.Interfaces;
using UIOMatic.Attributes;
using UIOMatic.Models;
using Umbraco.Core;

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
            return (IEnumerable<Type>)HttpRuntime.Cache["UIOMaticFolderTypes"] ?? EnsureUIOMaticTypes();
        }

        private static IEnumerable<Type> EnsureUIOMaticTypes()
        {
            var typesWithMyAttribute =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(UIOMaticFolderAttribute), true)
                where attributes != null && attributes.Length > 0
                select t; // UIOMaticFolderAttribute is the base type for all UIOMatic entities

            // Ensure unique aliases
            var aliases = new List<string>();
            foreach (var attr in typesWithMyAttribute.Select(type => type.GetCustomAttribute<UIOMaticFolderAttribute>(true)))
            {
                if (aliases.Any(x => x == attr.Alias))
                    throw new ApplicationException("Multiple UI-O-Matic model types found with alias '" + attr.Alias + "'. Please ensure all types have a unique alias value.");

                aliases.Add(attr.Alias);
            }

            HttpRuntime.Cache.Insert("UIOMaticFolderTypes", typesWithMyAttribute);
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
    }
}