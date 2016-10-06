using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using UIOmatic.Extensions;
using UIOMatic;
using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

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
            // Get needed resource
            var tableName = type.GetTableName(true);
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var db = GetDb(attri.ConnectionStringName);

            // Build up query
            var query = new Sql().Select("*").From(tableName);
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query.OrderBy(sortColumn.MakeSqlSafeName() + " " + sortOrder);
            }

            // Perform lookup
            return db.Fetch(type, query);
        }

        public UIOMaticPagedResult GetPaged(Type type, int itemsPerPage, int pageNumber, string sortColumn,
            string sortOrder, string searchTerm)
        {
            var tableName = (TableNameAttribute)Attribute.GetCustomAttribute(type, typeof(TableNameAttribute));
            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(type, typeof(UIOMaticAttribute));

            var db = !string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName)
                ? new Database(uioMaticAttri.ConnectionStringName)
                : ApplicationContext.Current.DatabaseContext.Database;

            var query = new Sql().Select("*").From(tableName.Value);

            var a1 = new QueryEventArgs(type, tableName.Value, query, sortColumn, sortOrder, searchTerm);
            UIOMaticObjectService.OnBuildingQuery(this, a1);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                int c = 0;
                foreach (var property in type.GetProperties())
                {
                    var attris = property.GetCustomAttributes();

                    if (!attris.Any(x=>x.GetType() == typeof(IgnoreAttribute)))
                    {
                        var before = "WHERE";
                        if (c > 0)
                            before = "OR";

                        var columnAttri =
                           attris.Where(x => x.GetType() == typeof(ColumnAttribute));

                        var columnName = property.Name;
                        if (columnAttri.Any())
                            columnName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;

                        query.Append(before + " [" + columnName + "] like @0", "%" + searchTerm + "%");
                        c++;

                    }
                }
            }
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
                query.OrderBy(sortColumn + " " + sortOrder);
            else if(!string.IsNullOrEmpty(uioMaticAttri.SortColumn) && !string.IsNullOrEmpty(uioMaticAttri.SortOrder))
            {
                query.OrderBy(uioMaticAttri.SortColumn + " " + uioMaticAttri.SortOrder);
            }
            else
            {
                var primaryKeyColum = "id";

                var primKeyAttri = type.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
                if (primKeyAttri.Any())
                    primaryKeyColum = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;

                foreach (var property in type.GetProperties())
                {
                    var keyAttri = property.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyColumnAttribute));
                    if (keyAttri.Any())
                        primaryKeyColum = property.Name;
                }

                query.OrderBy(primaryKeyColum + " asc");
            }
            
            var a2 = new QueryEventArgs(type, tableName.Value, query,sortColumn,sortOrder,searchTerm);
            UIOMaticObjectService.OnBuiltQuery(this, a2);

            var p = db.Page<dynamic>(pageNumber, itemsPerPage, a2.Query);
            var result = new UIOMaticPagedResult
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages
            };
            var items  = new List<object>();

            foreach (dynamic item in p.Items)
            {
                // get settable public properties of the type
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetSetMethod() != null);

                // create an instance of the type
                var obj = Activator.CreateInstance(type);


                // set property values using reflection
                var values = (IDictionary<string, object>)item;
                foreach (var prop in props)
                {
                    var columnAttri =
                           prop.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                    var propName = prop.Name;
                    if (columnAttri.Any())
                        propName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;

                    if(values.ContainsKey(propName))
                        prop.SetValue(obj, values[propName]);
                }

                items.Add(obj);
            }
            result.Items = items;
            return result;
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
                            var attri =
                                (UIOMaticFieldAttribute)
                                    attris.SingleOrDefault(x => x.GetType() == typeof (UIOMaticFieldAttribute));

                            var key = prop.Name;
                          
                            var view = attri.GetView();
                            if (prop.PropertyType == typeof(bool) && attri.View == "textfield")
                                view = "~/App_Plugins/UIOMatic/Backoffice/Views/checkbox.html";
                            if (prop.PropertyType == typeof(DateTime) && attri.View == "textfield")
                                view = "~/App_Plugins/UIOMatic/Backoffice/Views/datetime.html";
                            if ((prop.PropertyType == typeof(int) | prop.PropertyType == typeof(long)) && attri.View == "textfield")
                                view = "~/App_Plugins/UIOMatic/Backoffice/Views/number.html";
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
                        else
                        {
                            var key = prop.Name;
                           
                            string view = "~/App_Plugins/UIOMatic/Backoffice/Views/textfield.html";
                            if(prop.PropertyType == typeof(bool))
                                view = "~/App_Plugins/UIOMatic/Backoffice/Views/checkbox.html";
                            if (prop.PropertyType == typeof(DateTime))
                                view = "~/App_Plugins/UIOMatic/Backoffice/Views/datetime.html";
                            if (prop.PropertyType == typeof(int) | prop.PropertyType == typeof(long))
                                view = "~/App_Plugins/UIOMatic/Backoffice/Views/number.html";
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
                var attris = prop.GetCustomAttributes().ToArray();
                if (attris.All(x => !(x is IgnoreAttribute)))
                {
                    var colName = prop.Name;

                    var colAttri = attris.First(x => x is ColumnAttribute) as ColumnAttribute;
                    if (colAttri != null)
                        colName = colAttri.Name;

                    yield return colName;
                }
            }

        }

        public UIOMaticTypeInfo GetType(Type type)
        {
            var tableName = (TableNameAttribute)Attribute.GetCustomAttribute(type, typeof(TableNameAttribute));
            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(type, typeof(UIOMaticAttribute));

            var ignoreColumnsFromListView = new List<string>();
            var nameField = "";

            var primaryKey = "id";
            var primKeyAttri = type.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
            if (primKeyAttri.Any())
                primaryKey = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;

            foreach (var property in type.GetProperties())
            {
                var keyAttri = property.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyColumnAttribute));
                if (keyAttri.Any())
                    primaryKey = property.Name;

                var ignoreAttri = property.GetCustomAttributes().Where(x => x.GetType() == typeof(UIOMaticIgnoreFromListViewAttribute));
                if (ignoreAttri.Any())
                    ignoreColumnsFromListView.Add(property.Name);

                var nameAttri = property.GetCustomAttributes().Where(x => x.GetType() == typeof(UIOMaticNameFieldAttribute));
                if (nameAttri.Any())
                    nameField = property.Name;
            }

            return new UIOMaticTypeInfo()
            {
                RenderType = uioMaticAttri.RenderType,
                PrimaryKeyColumnName = primaryKey,
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

            return obj;
        }

        public object GetById(Type type, string id)
        {
            var tableName = ((TableNameAttribute)Attribute.GetCustomAttribute(type, typeof(TableNameAttribute))).Value;

            var primaryKeyColum = "id";

            var primKeyAttri = type.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
            if (primKeyAttri.Any())
                primaryKeyColum = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;

            foreach (var property in type.GetProperties())
            {
                var keyAttri = property.GetCustomAttributes().Where(x => x.GetType() == typeof (PrimaryKeyColumnAttribute));
                if (keyAttri.Any())
                    primaryKeyColum = property.Name;
            }

            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(type, typeof(UIOMaticAttribute));

            var db = !string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName)
                ? new Database(uioMaticAttri.ConnectionStringName)
                : ApplicationContext.Current.DatabaseContext.Database;

            var dyn = db.Query<dynamic>(Sql.Builder
                .Append("SELECT * FROM [" + tableName +"]")
                .Append("WHERE ["+primaryKeyColum+"] =@0", id));

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   .Where(x => x.GetSetMethod() != null);

            // create an instance of the type
            var obj = Activator.CreateInstance(type);


            // set property values using reflection
            var values = (IDictionary<string, object>)dyn.FirstOrDefault();
            foreach (var prop in props)
            {
                var columnAttri =
                       prop.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                var propName = prop.Name;
                if (columnAttri.Any())
                    propName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;
                if(values.ContainsKey(propName))
                    prop.SetValue(obj, values[propName]);
            }

            return obj;

           
        }

        public object Create(Type type, IDictionary<string, object> values)
        {
            var obj = Activator.CreateInstance(type, null);

            foreach (var prop in values)
            {
                if (prop.Value != null)
                {
                    var propKey = prop.Key;
                   
                    var propI = type.GetProperty(propKey);
                    if (propI != null)
                    {
                        obj.SetPropertyValue(propI.Name, prop.Value);
                    }

                }
            }


            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(type, typeof(UIOMaticAttribute));

            var db = !string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName)
                ? new Database(uioMaticAttri.ConnectionStringName)
                : ApplicationContext.Current.DatabaseContext.Database;

            var tableName = ((TableNameAttribute)Attribute.GetCustomAttribute(type, typeof(TableNameAttribute))).Value;

            var primaryKeyColum = string.Empty;
            var autoIncrement = true;

            var primKeyAttri = type.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
            if (primKeyAttri.Any())
            {
                primaryKeyColum = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;
                autoIncrement = ((PrimaryKeyAttribute)primKeyAttri.First()).autoIncrement;
            }

            foreach (var prop in type.GetProperties())
            {
                foreach (var attri in prop.GetCustomAttributes(true))
                {
                    if (attri.GetType() == typeof(PrimaryKeyColumnAttribute))
                    {
                        primaryKeyColum = ((PrimaryKeyColumnAttribute)attri).Name ?? prop.Name;
                        autoIncrement = ((PrimaryKeyColumnAttribute)attri).AutoIncrement;
                    }
                }


            }


            var a1 = new ObjectEventArgs(obj);
            UIOMaticObjectService.OnCreatingObject(this, a1);

            if (autoIncrement)
                db.Insert(tableName,primaryKeyColum, obj);
            else
                db.Insert(obj);

            var a2 = new ObjectEventArgs(obj);
            UIOMaticObjectService.OnCreatingObject(this, a2);

            return obj;

        }

        public object Update(Type type, IDictionary<string, object> values)
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


            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(type, typeof(UIOMaticAttribute));

            var db = !string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName)
                ? new Database(uioMaticAttri.ConnectionStringName)
                : ApplicationContext.Current.DatabaseContext.Database;

            var tableName = ((TableNameAttribute)Attribute.GetCustomAttribute(type, typeof(TableNameAttribute))).Value;

            var primaryKeyColum = string.Empty;

            var primKeyAttri = type.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
            if (primKeyAttri.Any())
                primaryKeyColum = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;

            foreach (var prop in type.GetProperties())
            {
                foreach (var attri in prop.GetCustomAttributes(true))
                {
                    if (attri.GetType() == typeof(PrimaryKeyColumnAttribute))
                        primaryKeyColum = ((PrimaryKeyColumnAttribute)attri).Name ?? prop.Name;

                }


            }

            var a1 = new ObjectEventArgs(obj);
            UIOMaticObjectService.OnUpdatingObject(this, a1);

            db.Update(obj);

            var a2 = new ObjectEventArgs(obj);
            UIOMaticObjectService.OnUpdatedObject(this, a2);

            return obj;
        }

        public string[] DeleteByIds(Type type, string ids)
        {
            var tableName = ((TableNameAttribute)Attribute.GetCustomAttribute(type, typeof(TableNameAttribute))).Value;
            
            var primaryKeyColum = string.Empty;

            var primKeyAttri = type.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
            if (primKeyAttri.Any())
                primaryKeyColum = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;

            foreach (var prop in type.GetProperties())
            {
                foreach (var attri in prop.GetCustomAttributes(true))
                {
                    if (attri.GetType() == typeof (PrimaryKeyColumnAttribute))
                        primaryKeyColum = ((PrimaryKeyColumnAttribute)attri).Name ?? prop.Name;

                }
                
                
            }

            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(type, typeof(UIOMaticAttribute));

            var db = !string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName)
                ? new Database(uioMaticAttri.ConnectionStringName)
                : ApplicationContext.Current.DatabaseContext.Database;

            //// TODO: Delete with one SQL statement?
            //var deletedIds = new List<string>();
            //foreach (var idStr in ids.Split(','))
            //{
            //    var id = 0;
            //    if (int.TryParse(idStr, out id))
            //    {
            //        deletedIds.Add(db.Delete(tableName, primaryKeyColum, null, id));
            //    }
            //}
            //return deletedIds.ToArray();

            string ids2var = "'" + ids.Replace(",", "','") + "'";
            string DEL_SQL = @"Delete from {0} where {1} in ({2})";
            DEL_SQL = string.Format(DEL_SQL, tableName, primaryKeyColum, ids2var);
            db.Execute(DEL_SQL);

           return ids.Split(',');
        }

        public IEnumerable<Exception> Validate(Type type, IDictionary<string, object> values)
        {
            var obj = Activator.CreateInstance(type, null);
            
            foreach (var prop in type.GetProperties())
            {
                var propKey = prop.Name;

                if (values.ContainsKey(propKey))
                {
                    obj.SetPropertyValue(prop.Name, values[propKey]);
                }
            }

            return ((IUIOMaticModel)obj).Validate();
        }

        public IEnumerable<object> GetFiltered(Type type, string filterColumn, string filterValue, string sortColumn, string sortOrder)
        {
            var tableName = (TableNameAttribute)Attribute.GetCustomAttribute(type, typeof(TableNameAttribute));
            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(type, typeof(UIOMaticAttribute));

            var db = !string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName)
                ? new Database(uioMaticAttri.ConnectionStringName)
                : ApplicationContext.Current.DatabaseContext.Database;

            var query = new Sql().Select("*").From(tableName.Value);

            query.Append("where" + "[" + filterColumn + "] = @0", filterValue);

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
                query.OrderBy(sortColumn + " " + sortOrder);

            foreach (dynamic item in db.Fetch<dynamic>(query))
            {
                // get settable public properties of the type
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetSetMethod() != null);

                // create an instance of the type
                var obj = Activator.CreateInstance(type);


                // set property values using reflection
                var values = (IDictionary<string, object>)item;
                foreach (var prop in props)
                {
                    var columnAttri =
                           prop.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                    var propName = prop.Name;
                    if (columnAttri.Any())
                        propName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;
                    if (values.ContainsKey(propName))
                        prop.SetValue(obj, values[propName]);
                }

                yield return obj;
            }
        }
    }
}