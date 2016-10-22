using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UIOmatic.Data;
using UIOmatic.Extensions;
using UIOmatic.Interfaces;
using UIOMatic.Attributes;
using UIOMatic.Models;
using Umbraco.Core;

namespace UIOMatic
{
    public class Helper
    {
        public static IUIOMaticRepository GetRepository(UIOMaticAttribute attr, UIOMaticTypeInfo typeInfo)
        {
            return attr.RepositoryType == typeof(DefaultUIOMaticRepository)
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
            var t = TypeFinder.FindClassesWithAttribute<UIOMaticFolderAttribute>(); // UIOMaticFolderAttribute is the base type for all UIOmatic entities
            HttpRuntime.Cache.Insert("UIOMaticFolderTypes", t);
            return t;
        }

        internal static Type GetUIOMaticTypeByAlias(string typeAlias, bool includeFolders = false, bool throwNullError = false)
        {
            var t = (includeFolders ? GetUIOMaticFolderTypes() : GetUIOMaticTypes()).FirstOrDefault(x => {
                var attr = x.GetCustomAttribute<UIOMaticFolderAttribute>(true);  // UIOMaticFolderAttribute is the base type for all UIOmatic entities
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