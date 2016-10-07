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
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var db = GetDb(attri.ConnectionStringName);

            var typeInfo = GetTypeInfo(type);
            var query = new Sql().Select("*").From(typeInfo.TableName);
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query.OrderBy(sortColumn + " " + sortOrder);
            }
            
            return db.Fetch(type, query);
        }

        public UIOMaticPagedResult GetPaged(Type type, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, string searchTerm)
        {
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var db = GetDb(attri.ConnectionStringName);

            var typeInfo = GetTypeInfo(type);
            var query = new Sql().Select("*").From(typeInfo.TableName);

            var a1 = new QueryEventArgs(type, typeInfo.TableName, query, sortColumn, sortOrder, searchTerm);
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

                        query.Append(before + " " + columnName + " like @0", "%" + searchTerm + "%");
                        c++;

                    }
                }
            }

            // Sort
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query.OrderBy(sortColumn + " " + sortOrder);
            }
            else if (!string.IsNullOrEmpty(attri.SortColumn) && !string.IsNullOrEmpty(attri.SortOrder))
            {
                query.OrderBy(attri.SortColumn + " " + attri.SortOrder);
            }
            else
            {
                var primaryKeyColum = type.GetPrimaryKeyName();
                query.OrderBy(primaryKeyColum + " asc");
            }

            var a2 = new QueryEventArgs(type, typeInfo.TableName, query,sortColumn,sortOrder,searchTerm);
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

        public UIOMaticTypeInfo GetTypeInfo(Type type, bool populateProperties =  false)
        {
            // Types shouldn't change without an app pool recycle so might as well cache these
            return (UIOMaticTypeInfo)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem("PetaPocoObjectService_GetTypeInfo_" + type.AssemblyQualifiedName + "_" + populateProperties, () =>
            {
                var attri = type.GetCustomAttribute<UIOMaticAttribute>();

                var properties = new List<UIOMaticPropertyInfo>();
                var listViewProperties = new List<UIOMaticPropertyInfo>();

                var nameField = "";

                foreach (var prop in type.GetProperties())
                {
                    var attris = prop.GetCustomAttributes();

                    if (populateProperties)
                    {
                        // Check for regular properties
                        var attri2 = attris.FirstOrDefault(x => x.GetType() == typeof(UIOMaticFieldAttribute)) as UIOMaticFieldAttribute;
                        if (attri2 != null)
                        {
                            var view = attri2.GetView();

                            // If field was left as textfield, see if we have a better match based on type
                            if (attri2.View == "textfield")
                            {
                                if (prop.PropertyType == typeof(bool)) view = Constants.Views["checkbox"];
                                if (prop.PropertyType == typeof(DateTime)) view = Constants.Views["datetime"];
                                if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long)) view = Constants.Views["number"];
                            }

                            var pi = new UIOMaticPropertyInfo
                            {
                                Key = prop.Name,
                                Name = attri2.Name.IsNullOrWhiteSpace() ? prop.Name : attri2.Name,
                                Tab = attri2.Tab.IsNullOrWhiteSpace() ? "Misc" : attri2.Tab,
                                Description = attri2.Description,
                                View = IOHelper.ResolveUrl(view),
                                Type = prop.PropertyType.ToString(),
                                Config = attri2.Config.IsNullOrWhiteSpace() ? null : (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(attri2.Config)
                            };

                            properties.Add(pi);
                        }
                        else
                        {
                            //TODO: If someone needs to re-instate supporting non-attributed properties, logic to handle these should be added here
                        }

                        // Check for list view properties
                        var attri3 = attris.FirstOrDefault(x => x.GetType() == typeof(UIOMaticListViewFieldAttribute)) as UIOMaticListViewFieldAttribute;
                        if (attri3 != null)
                        {
                            var view = attri3.GetView();

                            // Handle custom views?

                            var pi = new UIOMaticPropertyInfo
                            {
                                Key = prop.Name,
                                Name = attri3.Name.IsNullOrWhiteSpace() ? prop.Name : attri3.Name,
                                View = IOHelper.ResolveUrl(view),
                                Type = prop.PropertyType.ToString(),
                                Config = attri3.Config.IsNullOrWhiteSpace() ? null : (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(attri3.Config)
                            };

                            listViewProperties.Add(pi);
                        }
                    }

                    // Check for name field
                    var nameAttri = prop.GetCustomAttribute<UIOMaticNameFieldAttribute>();
                    if (nameAttri != null)
                        nameField = prop.Name;
                }

                return new UIOMaticTypeInfo
                {
                    TypeAlias = attri.Alias,
                    TableName = type.GetTableName(),
                    RenderType = attri.RenderType,
                    PrimaryKeyColumnName = type.GetPrimaryKeyName(),
                    AutoIncrementPrimaryKey = type.AutoIncrementPrimaryKey(),
                    NameField = nameField,
                    ReadOnly = attri.ReadOnly,
                    Properties = properties.ToArray(),
                    ListViewProperties = listViewProperties.ToArray()
                };
            });
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

            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var db = GetDb(attri.ConnectionStringName);

            var typeInfo = GetTypeInfo(type);
            var a1 = new ObjectEventArgs(obj);
            UIOMaticObjectService.OnCreatingObject(this, a1);

            if (typeInfo.AutoIncrementPrimaryKey)
                db.Insert(typeInfo.TableName, typeInfo.PrimaryKeyColumnName, true, obj);
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
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var db = GetDb(attri.ConnectionStringName);

            var typeInfo = GetTypeInfo(type);
            var sql = string.Format("DELETE FROM {0} WHERE {1} IN ({2})",
                typeInfo.TableName,
                typeInfo.PrimaryKeyColumnName,
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
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var db = GetDb(attri.ConnectionStringName);

            var typeInfo = GetTypeInfo(type);
            var query = new Sql().Select("*").From(typeInfo.TableName);

            if (!filterColumn.IsNullOrWhiteSpace())
            {
                query.Append("WHERE" + filterColumn + " = @0", filterValue);
            }

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query.OrderBy(sortColumn + " " + sortOrder);
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