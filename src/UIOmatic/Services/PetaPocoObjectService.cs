using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UIOmatic.Extensions;
using UIOMatic;
using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Persistence;
using Constants = UIOMatic.Constants;

namespace UIOmatic.Services
{
    
    public class PetaPocoObjectService : IUIOMaticObjectService
    {
        private Database GetDb(string connStr)
        {
            return !string.IsNullOrEmpty(connStr)
                ? new Database(connStr)
                : ApplicationContext.Current.DatabaseContext.Database;
        }

        public IEnumerable<object> GetAll(Type type, string sortColumn, string sortOrder)
        {
            var tableName = type.GetTableName();
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var db = GetDb(attri.ConnectionStringName);
            
            var query = new Sql().Select("*").From(tableName.MakeSqlSafeName());
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query.OrderBy(sortColumn.MakeSqlSafeName() + " " + sortOrder);
            }
            
            return db.Fetch(type, query);
        }

        public UIOMaticPagedResult GetPaged(Type type, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, string searchTerm)
        {
            var tableName = type.GetTableName();
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var db = GetDb(attri.ConnectionStringName);

            var query = new Sql().Select("*").From(tableName.MakeSqlSafeName());

            var a1 = new QueryEventArgs(type, tableName, query, sortColumn, sortOrder, searchTerm);
            UIOMaticObjectService.OnBuildingQuery(this, a1);
            query = a1.Query;

            // Filter by search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var c = 0;
                foreach (var property in type.GetProperties())
                {
                    var attris = property.GetCustomAttributes();
                    if (!attris.Any(x=>x.GetType() == typeof(IgnoreAttribute)))
                    {
                        var before = "WHERE";
                        if (c > 0)
                            before = "OR";

                        var columnName = property.Name;

                        var columnAttri = attris.FirstOrDefault(x => x.GetType() == typeof(ColumnAttribute)) as ColumnAttribute;
                        if (columnAttri != null)
                            columnName = columnAttri.Name;

                        query.Append(before + " " + columnName.MakeSqlSafeName() + " like @0", "%" + searchTerm + "%");
                        c++;

                    }
                }
            }

            // Sort
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query.OrderBy(sortColumn.MakeSqlSafeName() + " " + sortOrder);
            }
            else if (!string.IsNullOrEmpty(attri.SortColumn) && !string.IsNullOrEmpty(attri.SortOrder))
            {
                query.OrderBy(attri.SortColumn.MakeSqlSafeName() + " " + attri.SortOrder);
            }
            else
            {
                var primaryKeyColum = type.GetPrimaryKeyName();
                query.OrderBy(primaryKeyColum.MakeSqlSafeName() + " asc");
            }

            var a2 = new QueryEventArgs(type, tableName, query,sortColumn,sortOrder,searchTerm);
            UIOMaticObjectService.OnBuiltQuery(this, a2);
            query = a2.Query;

            var p = db.Page(type, pageNumber, itemsPerPage, query);

            return new UIOMaticPagedResult
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items
            };
        }

        public IEnumerable<UIOMaticPropertyInfo> GetAllProperties(Type type, bool includeIgnored = false)
        {
            foreach (var prop in type.GetProperties())
            {
                var attris = prop.GetCustomAttributes();

                if (includeIgnored || attris.All(x => x.GetType() != typeof(UIOMaticIgnoreFieldAttribute)))
                {
                    if (attris.Any(x => x.GetType() == typeof (UIOMaticFieldAttribute)))
                    {
                        var attri = attris.FirstOrDefault(x => x.GetType() == typeof (UIOMaticFieldAttribute)) as UIOMaticFieldAttribute;
                        if(attri != null)
                        { 
                            var key = prop.Name;
                            var view = attri.GetView();

                            // If field was left as textfield, see if we have a better match based on type
                            if (attri.View == "textfield")
                            {
                                if (prop.PropertyType == typeof(bool)) view = Constants.Views["checkbox"];
                                if (prop.PropertyType == typeof(DateTime)) view = Constants.Views["datetime"];
                                if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long)) view = Constants.Views["number"];
                            }

                            var pi = new UIOMaticPropertyInfo
                            {
                                Key = key,
                                Name = attri.Name,
                                Tab = string.IsNullOrEmpty(attri.Tab) ? "Misc" : attri.Tab,
                                Description = attri.Description,
                                View = IOHelper.ResolveUrl(view),
                                Type = prop.PropertyType.ToString() ,
                                Config = string.IsNullOrEmpty(attri.Config) ? null : (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(attri.Config)
                            };

                            yield return pi;
                        }
                    }
                    else
                    {
                        var key = prop.Name;

                        var view = Constants.Views["textfield"];

                        // See if we have a better view match based on type
                        if (prop.PropertyType == typeof(bool)) view = Constants.Views["checkbox"];
                        if (prop.PropertyType == typeof(DateTime)) view = Constants.Views["datetime"];
                        if (prop.PropertyType == typeof(int) | prop.PropertyType == typeof(long)) view = Constants.Views["number"];

                        var pi = new UIOMaticPropertyInfo
                        {
                            Key = key,
                            Name = prop.Name,
                            Tab = "Misc",
                            Description = string.Empty,
                            View = IOHelper.ResolveUrl(view),
                            Type = prop.PropertyType.ToString()
                               
                        };

                        yield return pi;
                    }
                }

                
            }

        }

        public IEnumerable<string> GetAllColumns(Type type)
        {
            foreach (var prop in type.GetProperties())
            {
                var attri = prop.GetCustomAttribute<IgnoreAttribute>();
                if (attri == null)
                {
                    yield return prop.GetColumnName();
                }
            }

        }

        public UIOMaticTypeInfo GetType(Type type)
        {
            var uioMaticAttri = type.GetCustomAttribute<UIOMaticAttribute>(); 
            var ignoreColumnsFromListView = new List<string>();

            var nameField = "";

            foreach (var property in type.GetProperties())
            {
                var ignoreAttri = property.GetCustomAttribute<UIOMaticIgnoreFromListViewAttribute>();
                if (ignoreAttri != null)
                    ignoreColumnsFromListView.Add(property.Name);

                var nameAttri = property.GetCustomAttribute<UIOMaticNameFieldAttribute>();
                if (nameAttri != null)
                    nameField = property.Name;
            }

            return new UIOMaticTypeInfo
            {
                RenderType = uioMaticAttri.RenderType,
                PrimaryKeyColumnName = type.GetPrimaryKeyName(),
                IgnoreColumnsFromListView = ignoreColumnsFromListView.ToArray(),
                NameField = nameField,
                ReadOnly = uioMaticAttri.ReadOnly
            };
        }

        public object GetScaffold(Type type)
        {
            var obj = Activator.CreateInstance(type);

            var a1 = new ObjectEventArgs(obj);
            UIOMaticObjectService.OnScaffoldingObject(this, a1);

            return a1.Object;
        }

        public object GetById(Type type, string id)
        {
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var db = GetDb(attri.ConnectionStringName);

            return db.SingleOrDefault(type, id);
        }

        public object Create(Type type, IDictionary<string, object> values)
        {
            var obj = CreateAndPopulateType(type, values);

            var tableName = type.GetTableName();
            var primaryKeyColum = type.GetPrimaryKeyName();
            var autoIncrement = type.AutoIncrementPrimaryKey();

            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var db = GetDb(attri.ConnectionStringName);

            var a1 = new ObjectEventArgs(obj);
            UIOMaticObjectService.OnCreatingObject(this, a1);

            if (autoIncrement)
                db.Insert(tableName, primaryKeyColum, true, obj);
            else
                db.Insert(obj);

            var a2 = new ObjectEventArgs(obj);
            UIOMaticObjectService.OnCreatingObject(this, a2);

            return obj;

        }

        public object Update(Type type, IDictionary<string, object> values)
        {
            var obj = CreateAndPopulateType(type, values);
            
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var db = GetDb(attri.ConnectionStringName);

            var a1 = new ObjectEventArgs(obj);
            UIOMaticObjectService.OnUpdatingObject(this, a1);

            db.Update(obj);

            var a2 = new ObjectEventArgs(obj);
            UIOMaticObjectService.OnUpdatedObject(this, a2);

            return obj;
        }

        public string[] DeleteByIds(Type type, string[] ids)
        {
            var tableName = type.GetTableName();
            var primaryKeyColum = type.GetPrimaryKeyName();

            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var db = GetDb(attri.ConnectionStringName);
            
            var sql = string.Format("DELETE FROM {0} WHERE {1} IN ({2})", 
                tableName.MakeSqlSafeName(), 
                primaryKeyColum.MakeSqlSafeName(),
                ids.Select(x => "'" + x + "'").ToArray());

            db.Execute(sql);

           return ids;
        }

        public IEnumerable<Exception> Validate(Type type, IDictionary<string, object> values)
        {
            var obj = CreateAndPopulateType(type, values);
            return ((IUIOMaticModel)obj).Validate();
        }

        public IEnumerable<object> GetFiltered(Type type, string filterColumn, string filterValue, string sortColumn, string sortOrder)
        {
            var tableName = type.GetTableName();

            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var db = GetDb(attri.ConnectionStringName);

            var query = new Sql().Select("*").From(tableName.MakeSqlSafeName());

            if (!filterColumn.IsNullOrWhiteSpace())
            {
                query.Append("WHERE" + filterColumn.MakeSqlSafeName() + " = @0", filterValue);
            }

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query.OrderBy(sortColumn.MakeSqlSafeName() + " " + sortOrder);
            }

            return db.Fetch(type, query);
        }

        private object CreateAndPopulateType(Type type, IDictionary<string, object> values)
        {
            var obj = Activator.CreateInstance(type, null);

            foreach (var prop in values)
            {
                var propKey = prop.Key;

                var propI = type.GetProperty(propKey);
                if (propI != null)
                {
                    obj.SetPropertyValue(propI.Name, prop.Value);
                }
            }

            return obj;
        }
    }
}