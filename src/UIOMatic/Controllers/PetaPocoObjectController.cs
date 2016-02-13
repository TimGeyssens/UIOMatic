using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Core.IO;
using Umbraco.Core;

namespace UIOMatic.Controllers
{

    public class PetaPocoObjectController : UmbracoAuthorizedJsonController, IUIOMaticObjectController
    {
        public static event EventHandler<QueryEventArgs> BuildingQuery;
        public static event EventHandler<QueryEventArgs> BuildedQuery;

        public static event EventHandler<ObjectEventArgs> UpdatingObject;
        public static event EventHandler<ObjectEventArgs> UpdatedObject;

        public static event EventHandler<ObjectEventArgs> CreatingObject;
        public static event EventHandler<ObjectEventArgs> CreatedObject;

        public IEnumerable<object> GetAll(string typeName, string sortColumn, string sortOrder)
        {
            var currentType = Type.GetType(typeName);
            var tableName = (TableNameAttribute)Attribute.GetCustomAttribute(currentType, typeof(TableNameAttribute));
            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(currentType, typeof(UIOMaticAttribute));
            string strTableName = tableName.Value;
            var db = (Database)DatabaseContext.Database;
            if (uioMaticAttri != null && !string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName))
                db = new Database(uioMaticAttri.ConnectionStringName);
            if (strTableName.IndexOf("[") < 0)
            {
                strTableName = "[" + strTableName + "]";
            }
            var query = new Sql().Select("*").From(strTableName);

            string strSortColumn = sortColumn;

            if (!string.IsNullOrEmpty(strSortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                if (strSortColumn.IndexOf("[") < 0)
                {
                    strSortColumn = "[" + strSortColumn + "]";
                }

                query.OrderBy(strSortColumn + " " + sortOrder);


            }

            foreach (dynamic item in db.Fetch<dynamic>(query))
            {
                // get settable public properties of the type
                var props = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetSetMethod() != null);

                // create an instance of the type
                var obj = Activator.CreateInstance(currentType);


                // set property values using reflection
                var values = (IDictionary<string, object>)item;
                foreach (var prop in props)
                {
                    var columnAttri =
                           prop.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                    string propName = "";
                    if (columnAttri.Any())
                        propName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;
                    if (string.IsNullOrWhiteSpace(propName))
                    {
                        propName = prop.Name;
                    }

                    //prop.SetValue(obj, values[propName]);
                    if (values.ContainsKey(propName))
                        prop.SetValue(obj, values[propName]);
                }

                yield return obj;
            }



        }

        public UIOMaticPagedResult GetPaged(string typeName, int itemsPerPage, int pageNumber, string sortColumn,
            string sortOrder, string searchTerm)
        {
            var currentType = Type.GetType(typeName);
            var tableName = (TableNameAttribute)Attribute.GetCustomAttribute(currentType, typeof(TableNameAttribute));
            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(currentType, typeof(UIOMaticAttribute));

            var db = (Database)DatabaseContext.Database;
            if (!string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName))
                db = new Database(uioMaticAttri.ConnectionStringName);

            var query = new Sql().Select("*").From(tableName.Value);

            EventHandler<QueryEventArgs> tmp = BuildingQuery;
            if (tmp != null)
                tmp(this, new QueryEventArgs(tableName.Value, query));

            if (!string.IsNullOrEmpty(searchTerm))
            {
                int c = 0;
                foreach (var property in currentType.GetProperties())
                {
                    //if (property.PropertyType == typeof (string))
                    //{
                    string before = "WHERE";
                    if (c > 0)
                        before = "OR";

                    var columnAttri =
                       property.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                    var columnName = property.Name;
                    if (columnAttri.Any())
                        columnName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;

                    if (string.IsNullOrWhiteSpace(columnName))
                    {
                        columnName = property.Name;
                    }

                    query.Append(before + " [" + columnName + "] like @0", "%" + searchTerm + "%");
                    c++;

                    //}
                }
            }
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
                query.OrderBy(sortColumn + " " + sortOrder);
            else
            {
                var primaryKeyColum = "id";

                var primKeyAttri = currentType.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
                if (primKeyAttri.Any())
                    primaryKeyColum = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;

                foreach (var property in currentType.GetProperties())
                {
                    var keyAttri = property.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyColumnAttribute));
                    if (keyAttri.Any())
                        primaryKeyColum = property.Name;
                }

                query.OrderBy(primaryKeyColum + " asc");
            }

            EventHandler<QueryEventArgs> temp = BuildedQuery;
            if (temp != null)
                temp(this, new QueryEventArgs(tableName.Value, query));

            var p = db.Page<dynamic>(pageNumber, itemsPerPage, query);
            var result = new UIOMaticPagedResult
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages
            };
            var items = new List<object>();

            foreach (dynamic item in p.Items)
            {
                // get settable public properties of the type
                var props = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetSetMethod() != null);

                // create an instance of the type
                var obj = Activator.CreateInstance(currentType);


                // set property values using reflection
                var values = (IDictionary<string, object>)item;
                foreach (var prop in props)
                {
                    var columnAttri =
                           prop.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                    var propName = prop.Name;
                    if (columnAttri.Any())
                        propName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;
                    if (string.IsNullOrWhiteSpace(propName))
                    {
                        propName = prop.Name;
                    }
                    prop.SetValue(obj, values[propName]);
                }

                items.Add(obj);
            }
            result.Items = items;
            return result;
        }

        public IEnumerable<UIOMaticPropertyInfo> GetAllProperties(string typeName, bool isEdit, bool includeIgnored = false)
        {
            var ar = typeName.Split(',');
            var currentType = Type.GetType(ar[0] + ", " + ar[1]);
            foreach (var prop in currentType.GetProperties())
            {

                var attris = prop.GetCustomAttributes();

                if (includeIgnored || attris.All(x => x.GetType() != typeof(UIOMaticIgnoreFieldAttribute)))
                {

                    if (attris.Any(x => x.GetType() == typeof(UIOMaticFieldAttribute)))
                    {
                        var attri =
                            (UIOMaticFieldAttribute)
                                attris.SingleOrDefault(x => x.GetType() == typeof(UIOMaticFieldAttribute));

                        var key = prop.Name;

                        string view = attri.GetView();
                        if (prop.PropertyType == typeof(bool) && attri.View == "textfield")
                            view = "~/App_Plugins/UIOMatic/Backoffice/Views/checkbox.html";
                        if ((prop.PropertyType == typeof(DateTime) | prop.PropertyType == typeof(DateTime?)) && attri.View == "textfield")
                            view = "~/App_Plugins/UIOMatic/Backoffice/Views/datetime.html";
                        if ((prop.PropertyType == typeof(int) | prop.PropertyType == typeof(long)) && attri.View == "textfield")
                            view = "~/App_Plugins/UIOMatic/Backoffice/Views/number.html";
                        if (!attri.IsCanEdit && isEdit && attri.View != "file")
                        {
                            view = "~/App_Plugins/UIOMatic/Backoffice/Views/label.html";
                        }
                        var pi = new UIOMaticPropertyInfo
                        {
                            Key = key,
                            Name = attri.Name,
                            Tab = string.IsNullOrEmpty(attri.Tab) ? "Misc" : attri.Tab,
                            Description = attri.Description,
                            View = IOHelper.ResolveUrl(view),
                            Type = prop.PropertyType.ToString(),
                            Config = string.IsNullOrEmpty(attri.Config) ? null : (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(attri.Config),
                            IsReadOnly = (!attri.IsCanEdit && isEdit)
                        };
                        yield return pi;
                    }
                    else
                    {
                        var key = prop.Name;

                        string view = "~/App_Plugins/UIOMatic/Backoffice/Views/textfield.html";
                        if (prop.PropertyType == typeof(bool))
                            view = "~/App_Plugins/UIOMatic/Backoffice/Views/checkbox.html";
                        if (prop.PropertyType == typeof(DateTime) | prop.PropertyType == typeof(DateTime?))
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

        public IEnumerable<UIOMaticFilterPropertyInfo> GetFilterProperties(string typeName)
        {
            var ar = typeName.Split(',');
            var currentType = Type.GetType(ar[0] + ", " + ar[1]);
            foreach (var prop in currentType.GetProperties())
            {

                var attris = prop.GetCustomAttributes();

                if (attris.Any(x => x.GetType() == typeof(UIOMaticFilterFieldAttribute)))
                {
                    var filter =
                            (UIOMaticFilterFieldAttribute)
                                attris.SingleOrDefault(x => x.GetType() == typeof(UIOMaticFilterFieldAttribute));
                    filter.DefaultValue = Helper.HandleDefaultValue(filter.DefaultValue);
                    if (filter.DefaultToValue == "today")
                    {
                        filter.DefaultToValue = Helper.HandleDefaultValue(filter.DefaultToValue, 1);
                    }
                    else
                    {
                        filter.DefaultToValue = Helper.HandleDefaultValue(filter.DefaultToValue);
                    }


                    if (filter.ShowNumbers == 0)
                    {
                        filter.ShowNumbers = 1;
                    }
                    for (int i = 0; i < filter.ShowNumbers; i++)
                    {
                        if (attris.Any(x => x.GetType() == typeof(UIOMaticFieldAttribute)))
                        {
                            var attri =
                                (UIOMaticFieldAttribute)
                                    attris.SingleOrDefault(x => x.GetType() == typeof(UIOMaticFieldAttribute));

                            var key = prop.Name;

                            string view = attri.GetView();
                            if (prop.PropertyType == typeof(bool) && attri.View == "textfield")
                                view = "~/App_Plugins/UIOMatic/Backoffice/Views/checkbox.html";
                            if (prop.PropertyType == typeof(DateTime) && attri.View == "textfield")
                                view = "~/App_Plugins/UIOMatic/Backoffice/Views/daterange.html";
                            if ((prop.PropertyType == typeof(int) | prop.PropertyType == typeof(long)) && attri.View == "textfield")
                                view = "~/App_Plugins/UIOMatic/Backoffice/Views/number.html";
                            var pi = new UIOMaticFilterPropertyInfo
                            {
                                Key = key,
                                Name = attri.Name,
                                Tab = string.IsNullOrEmpty(attri.Tab) ? "Misc" : attri.Tab,
                                Description = attri.Description,
                                View = IOHelper.ResolveUrl(view),
                                Type = prop.PropertyType.ToString(),
                                Config = string.IsNullOrEmpty(attri.Config) ? null : (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(attri.Config),
                                OperatorID = "1"
                            };
                            if (!string.IsNullOrWhiteSpace(filter.OperatorCode))
                            {
                                pi.OperatorID = filter.OperatorCode;
                            }
                            if (!string.IsNullOrWhiteSpace(filter.DefaultValue))
                            {
                                pi.Value = filter.DefaultValue;
                            }
                            if (!string.IsNullOrWhiteSpace(filter.DefaultToValue))
                            {
                                pi.ToValue = filter.DefaultToValue;
                            }
                            yield return pi;
                        }
                        else
                        {
                            var key = prop.Name;

                            string view = "~/App_Plugins/UIOMatic/Backoffice/Views/textfield.html";
                            if (prop.PropertyType == typeof(bool))
                                view = "~/App_Plugins/UIOMatic/Backoffice/Views/checkbox.html";
                            if (prop.PropertyType == typeof(DateTime))
                                view = "~/App_Plugins/UIOMatic/Backoffice/Views/daterange.html";
                            if (prop.PropertyType == typeof(int) | prop.PropertyType == typeof(long))
                                view = "~/App_Plugins/UIOMatic/Backoffice/Views/number.html";
                            var pi = new UIOMaticFilterPropertyInfo
                            {
                                Key = key,
                                Name = prop.Name,
                                Tab = "Misc",
                                Description = string.Empty,
                                View = IOHelper.ResolveUrl(view),
                                Type = prop.PropertyType.ToString(),
                                OperatorID = "1"

                            };
                            if (!string.IsNullOrWhiteSpace(filter.OperatorCode))
                            {
                                pi.OperatorID = filter.OperatorCode;
                            }
                            if (!string.IsNullOrWhiteSpace(filter.DefaultValue))
                            {

                                pi.Value = filter.DefaultValue;
                            }
                            if (!string.IsNullOrWhiteSpace(filter.DefaultToValue))
                            {
                                pi.ToValue = filter.DefaultToValue;
                            }
                            yield return pi;
                        }
                    }

                }


            }

        }

        public IEnumerable<string> GetAllColumns(string typeName)
        {
            var ar = typeName.Split(',');
            var currentType = Type.GetType(ar[0] + ", " + ar[1]);
            foreach (var prop in currentType.GetProperties())
            {
                var attris = prop.GetCustomAttributes();

                if (attris.All(x => x.GetType() != typeof(IgnoreAttribute)))
                {
                    string colName = prop.Name;

                    if (attris.Any(x => x.GetType() == typeof(ColumnAttribute)))
                        colName = ((ColumnAttribute)attris.First(x => x.GetType() == typeof(ColumnAttribute))).Name;
                    if (string.IsNullOrWhiteSpace(colName))
                    {
                        colName = prop.Name;
                    }
                    yield return colName;
                }
            }

        }
        public UIOMaticTypeInfo GetType(string typeName)
        {
            var currentType = Type.GetType(typeName);
            var tableName = (TableNameAttribute)Attribute.GetCustomAttribute(currentType, typeof(TableNameAttribute));
            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(currentType, typeof(UIOMaticAttribute));

            var ignoreColumnsFromListView = new List<string>();
            //var filterColumnsFromListView = new List<string>();
            var nameField = "";

            var primaryKey = "id";
            var primKeyAttri = currentType.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
            if (primKeyAttri.Any())
                primaryKey = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;

            foreach (var property in currentType.GetProperties())
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

                //var filterAttri = property.GetCustomAttributes().Where(x => x.GetType() == typeof(UIOMaticFilterFieldAttribute));
                //if (filterAttri.Any())
                //    filterColumnsFromListView.Add(property.Name);

            }

            return new UIOMaticTypeInfo()
            {
                RenderType = uioMaticAttri.RenderType,
                PrimaryKeyColumnName = primaryKey,
                IgnoreColumnsFromListView = ignoreColumnsFromListView.ToArray(),
                NameField = nameField,
                DisplayName = uioMaticAttri.Name,
                QueryTemplate = IOHelper.ResolveUrl("~/App_Plugins/UIOMatic/backoffice/views/query.html"),
                IsCanExport = uioMaticAttri.IsCanExport,
                IsReadOnly = uioMaticAttri.ReadOnly,
                ShowInTree = uioMaticAttri.ShowInTree
            };
        }

        public object GetById(string typeName, string id)
        {


            var ar = typeName.Split(',');
            var currentType = Type.GetType(ar[0] + ", " + ar[1]);
            var tableName = ((TableNameAttribute)Attribute.GetCustomAttribute(currentType, typeof(TableNameAttribute))).Value;

            var primaryKeyColum = "id";

            var primKeyAttri = currentType.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
            if (primKeyAttri.Any())
                primaryKeyColum = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;

            foreach (var property in currentType.GetProperties())
            {
                var keyAttri = property.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyColumnAttribute));
                if (keyAttri.Any())
                    primaryKeyColum = property.Name;
            }

            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(currentType, typeof(UIOMaticAttribute));

            var db = (Database)DatabaseContext.Database;
            if (!string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName))
                db = new Database(uioMaticAttri.ConnectionStringName);

            var dyn = db.Query<dynamic>(Sql.Builder
                .Append("SELECT * FROM [" + tableName + "]")
                .Append("WHERE [" + primaryKeyColum + "] =@0", id));

            var props = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   .Where(x => x.GetSetMethod() != null);

            // create an instance of the type
            var obj = Activator.CreateInstance(currentType);


            // set property values using reflection
            var values = (IDictionary<string, object>)dyn.FirstOrDefault();
            foreach (var prop in props)
            {
                var columnAttri =
                       prop.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                var propName = prop.Name;
                if (columnAttri.Any())
                    propName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;
                if (string.IsNullOrWhiteSpace(propName))
                {
                    propName = prop.Name;
                }
                //prop.SetValue(obj, values[propName]);
                if (values.ContainsKey(propName))
                    prop.SetValue(obj, values[propName]);
            }

            return obj;


        }

        public object PostCreate(ExpandoObject objectToCreate)
        {
            var typeOfObject = objectToCreate.FirstOrDefault(x => x.Key == "typeOfObject").Value.ToString();
            objectToCreate = (ExpandoObject)objectToCreate.FirstOrDefault(x => x.Key == "objectToCreate").Value;

            var ar = typeOfObject.Split(',');
            var currentType = Type.GetType(ar[0] + ", " + ar[1]);

            object ob = Activator.CreateInstance(currentType, null);

            foreach (var prop in objectToCreate)
            {
                var propKey = prop.Key;
                PropertyInfo propI = currentType.GetProperty(propKey);
                if (prop.Value != null)
                {
                    Helper.SetValue(ob, propI.Name, prop.Value);

                }

            }


            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(currentType, typeof(UIOMaticAttribute));

            var db = (Database)DatabaseContext.Database;
            if (!string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName))
                db = new Database(uioMaticAttri.ConnectionStringName);

            var tableName = ((TableNameAttribute)Attribute.GetCustomAttribute(currentType, typeof(TableNameAttribute))).Value;

            var primaryKeyColum = string.Empty;

            var primKeyAttri = currentType.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
            if (primKeyAttri.Any())
            {
                primaryKeyColum = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;
                if (!((PrimaryKeyAttribute)primKeyAttri.First()).autoIncrement)
                {
                    PropertyInfo propI = currentType.GetProperty(primaryKeyColum);
                    if (propI.PropertyType == typeof(Guid))
                    {
                        Helper.SetValue(ob, propI.Name, Guid.NewGuid());
                    }
                }
            }
            else
            {
                foreach (var prop in currentType.GetProperties())
                {
                    foreach (var attri in prop.GetCustomAttributes(true))
                    {
                        if (attri.GetType() == typeof(PrimaryKeyColumnAttribute))
                        {
                            primaryKeyColum = ((PrimaryKeyColumnAttribute)attri).Name ?? prop.Name;
                            if (!((PrimaryKeyColumnAttribute)attri).AutoIncrement)
                            {
                                PropertyInfo propI = currentType.GetProperty(primaryKeyColum);
                                if (propI.PropertyType == typeof(Guid))
                                {
                                    Helper.SetValue(ob, propI.Name, Guid.NewGuid());
                                }
                            }
                        }
                    }


                }
            }


            //db.Save(tableName, primaryKeyColum, ob);

            EventHandler<ObjectEventArgs> temp = CreatingObject;
            if (temp != null)
                temp(this, new ObjectEventArgs(ob));

            ((IUIOMaticModel)ob).SetDefaultValue();
            db.Insert(ob);

            EventHandler<ObjectEventArgs> tmp = CreatedObject;
            if (tmp != null)
                tmp(this, new ObjectEventArgs(ob));

            return ob;

        }

        public object PostUpdate(ExpandoObject objectToUpdate)
        {
            var typeOfObject = objectToUpdate.FirstOrDefault(x => x.Key == "typeOfObject").Value.ToString();
            objectToUpdate = (ExpandoObject)objectToUpdate.FirstOrDefault(x => x.Key == "objectToUpdate").Value;

            var ar = typeOfObject.Split(',');
            var currentType = Type.GetType(ar[0] + ", " + ar[1]);

            object ob = Activator.CreateInstance(currentType, null);

            foreach (var prop in objectToUpdate)
            {
                var propKey = prop.Key;
                PropertyInfo propI = currentType.GetProperty(propKey);
                //object value
                if (prop.Value != null)
                {
                    Helper.SetValue(ob, propI.Name, prop.Value);

                }
            }


            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(currentType, typeof(UIOMaticAttribute));

            var db = (Database)DatabaseContext.Database;
            if (!string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName))
                db = new Database(uioMaticAttri.ConnectionStringName);

            var tableName = ((TableNameAttribute)Attribute.GetCustomAttribute(currentType, typeof(TableNameAttribute))).Value;

            var primaryKeyColum = string.Empty;

            var primKeyAttri = currentType.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
            if (primKeyAttri.Any())
                primaryKeyColum = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;

            foreach (var prop in currentType.GetProperties())
            {
                foreach (var attri in prop.GetCustomAttributes(true))
                {
                    if (attri.GetType() == typeof(PrimaryKeyColumnAttribute))
                        primaryKeyColum = ((PrimaryKeyColumnAttribute)attri).Name ?? prop.Name;

                }


            }

            EventHandler<ObjectEventArgs> tmp = UpdatingObject;
            if (tmp != null)
                tmp(this, new ObjectEventArgs(ob));

            ((IUIOMaticModel)ob).SetDefaultValue();
            db.Save(tableName, primaryKeyColum, ob);
            //db.Save(ob);
            //db.Update(ob);
            EventHandler<ObjectEventArgs> temp = UpdatedObject;
            if (temp != null)
                temp(this, new ObjectEventArgs(ob));


            return ob;
        }

        public string[] DeleteByIds(string typeOfObject, string ids)
        {
            var currentType = Helper.GetTypesWithUIOMaticAttribute().First(x => x.AssemblyQualifiedName.Contains(typeOfObject));
            var tableName = ((TableNameAttribute)Attribute.GetCustomAttribute(currentType, typeof(TableNameAttribute))).Value;

            var primaryKeyColum = string.Empty;

            var primKeyAttri = currentType.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
            if (primKeyAttri.Any())
                primaryKeyColum = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;

            foreach (var prop in currentType.GetProperties())
            {
                foreach (var attri in prop.GetCustomAttributes(true))
                {
                    if (attri.GetType() == typeof(PrimaryKeyColumnAttribute))
                        primaryKeyColum = ((PrimaryKeyColumnAttribute)attri).Name ?? prop.Name;

                }


            }

            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(currentType, typeof(UIOMaticAttribute));

            var db = (Database)DatabaseContext.Database;
            if (!string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName))
                db = new Database(uioMaticAttri.ConnectionStringName);

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

        [HttpPost]
        public IEnumerable<Exception> Validate(ExpandoObject objectToValidate)
        {
            var typeOfObject = objectToValidate.FirstOrDefault(x => x.Key == "typeOfObject").Value.ToString();
            objectToValidate = (ExpandoObject)objectToValidate.FirstOrDefault(x => x.Key == "objectToValidate").Value;

            var ar = typeOfObject.Split(',');
            var currentType = Type.GetType(ar[0] + ", " + ar[1]);


            object ob = Activator.CreateInstance(currentType, null);

            var values = (IDictionary<string, object>)objectToValidate;
            foreach (var prop in currentType.GetProperties())
            {
                var propKey = prop.Name;


                if (values.ContainsKey(propKey))
                {
                    Helper.SetValue(ob, prop.Name, values[propKey]);
                }
            }


            return ((IUIOMaticModel)ob).Validate();
        }
        private int GetCurrentUserId()
        {
            var userService = ApplicationContext.Current.Services.UserService;
            return userService.GetByUsername(HttpContext.Current.User.Identity.Name).Id;
        }

        public IEnumerable<object> GetFiltered(string typeName, string filterColumn, string filterValue, string sortColumn, string sortOrder)
        {
            var currentType = Type.GetType(typeName);
            var tableName = (TableNameAttribute)Attribute.GetCustomAttribute(currentType, typeof(TableNameAttribute));
            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(currentType, typeof(UIOMaticAttribute));

            var db = (Database)DatabaseContext.Database;
            if (!string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName))
                db = new Database(uioMaticAttri.ConnectionStringName);

            var query = new Sql().Select("*").From(tableName.Value);

            query.Append("where" + "[" + filterColumn + "] = @0", filterValue);

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
                query.OrderBy(sortColumn + " " + sortOrder);

            foreach (dynamic item in db.Fetch<dynamic>(query))
            {
                // get settable public properties of the type
                var props = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetSetMethod() != null);

                // create an instance of the type
                var obj = Activator.CreateInstance(currentType);


                // set property values using reflection
                var values = (IDictionary<string, object>)item;
                foreach (var prop in props)
                {
                    var columnAttri =
                           prop.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                    var propName = prop.Name;
                    if (columnAttri.Any())
                        propName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;
                    if (string.IsNullOrWhiteSpace(propName))
                    {
                        propName = prop.Name;
                    }
                    if (values.ContainsKey(propName))
                        prop.SetValue(obj, values[propName]);

                }

                yield return obj;
            }
        }

        public UIOMaticPagedResult GetQuery(UIOMaticQueryInfo queryinfo)
        {
            var currentType = Type.GetType(queryinfo.TypeName);
            var tableName = (TableNameAttribute)Attribute.GetCustomAttribute(currentType, typeof(TableNameAttribute));
            var uioMaticAttri = (UIOMaticAttribute)Attribute.GetCustomAttribute(currentType, typeof(UIOMaticAttribute));

            var db = (Database)DatabaseContext.Database;
            if (!string.IsNullOrEmpty(uioMaticAttri.ConnectionStringName))
                db = new Database(uioMaticAttri.ConnectionStringName);

            string strTableName = tableName.Value;
            if (strTableName.IndexOf("[") < 0)
            {
                strTableName = "[" + strTableName + "]";
            }
            var query = new Sql().Select("*").From(strTableName);

            EventHandler<QueryEventArgs> tmp = BuildingQuery;
            if (tmp != null)
                tmp(this, new QueryEventArgs(tableName.Value, query));


            if (queryinfo.FilterProperty != null)
            {
                foreach (var item in queryinfo.FilterProperty)
                {

                    if (!string.IsNullOrWhiteSpace(item.Value) || !string.IsNullOrWhiteSpace(item.ToValue))
                    {
                        string columnName = item.Key;
                        var prop = currentType.GetProperties().FirstOrDefault(x => x.Name == item.Key);
                        if (prop != null)
                        {
                            var columnAttri = prop.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                            if (columnAttri.Any())
                                columnName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;

                            if (string.IsNullOrWhiteSpace(columnName))
                            {
                                columnName = item.Key;
                            }
                        }

                        if (item.Type.ToLower().Contains("datetime"))
                        {
                            if (string.IsNullOrWhiteSpace(item.Value))
                            {
                                item.Value = DateTime.MinValue.ToString("yyy-MM-dd");
                            }

                            if (string.IsNullOrWhiteSpace(item.ToValue))
                            {
                                item.ToValue = DateTime.MaxValue.ToString("yyy-MM-dd");
                            }
                            query.Where("[" + columnName + "] Between @0 AND @1", item.Value, item.ToValue);
                        }
                        else if (item.Type.ToLower().Contains("bool"))
                        {
                            query.Where("[" + columnName + "] = @0", item.Value);
                        }
                        else if (item.Type.ToLower().Contains("string"))
                        {
                            query.Where("[" + columnName + "] like '%" + item.Value + "%'");
                        }
                        else
                        {
                            query.Where("[" + columnName + "] " + Helper.GetOperators(item.OperatorID) + " @0", item.Value);
                        }


                    }
                }
            }
            if (!string.IsNullOrEmpty(queryinfo.SearchTerm))
            {
                int c = 0;
                string search = "";
                foreach (var property in currentType.GetProperties())
                {
                    //if (property.PropertyType == typeof (string))
                    //{
                    var ig =
                       property.GetCustomAttributes().Where(x => x.GetType() == typeof(IgnoreAttribute));
                    if (ig.Any())
                    {
                        continue;
                    }
                    string before = "";
                    if (c > 0)
                        before = " OR ";

                    var columnAttri =
                       property.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                    var columnName = property.Name;
                    if (columnAttri.Any())
                        columnName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;

                    if (string.IsNullOrWhiteSpace(columnName))
                    {
                        columnName = property.Name;
                    }

                    search += before + " [" + columnName + "] like '%" + queryinfo.SearchTerm + "%' ";
                    c++;

                    //}
                }
                query.Where(search);
            }

            if (!string.IsNullOrEmpty(queryinfo.SortColumn) && !string.IsNullOrEmpty(queryinfo.SortOrder))
            {
                var sortcolumn = queryinfo.SortColumn;


                Dictionary<int, string> order = new Dictionary<int, string>();
                foreach (var property in currentType.GetProperties())
                {
                    var orderAttri = (UIOMaticSortOrderAttribute)property.GetCustomAttributes().FirstOrDefault(x => x.GetType() == typeof(UIOMaticSortOrderAttribute));
                    if (orderAttri != null)
                    {
                        var columnAttri =
                               property.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                        var propName = property.Name;
                        if (columnAttri.Any())
                            propName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;
                        if (string.IsNullOrWhiteSpace(propName))
                        {
                            propName = property.Name;
                        }

                        if (propName.IndexOf("[") < 0)
                        {
                            propName = "([" + propName + "])";
                        }
                        if (orderAttri.IsDescending)
                        {
                            propName += " desc";
                        }
                        order.Add(orderAttri.Sequence, propName);
                    }
                    if (sortcolumn == property.Name)
                    {
                        var columnAttri =
                               property.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                        //var propName = property.Name;
                        if (columnAttri.Any())
                            sortcolumn = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;
                        if (string.IsNullOrWhiteSpace(sortcolumn))
                        {
                            sortcolumn = property.Name;
                        }
                    }
                }
                if (sortcolumn.IndexOf("[") < 0)
                {
                    sortcolumn = "([" + sortcolumn + "])";
                }
                query.OrderBy(sortcolumn + " " + queryinfo.SortOrder);
                var list = order.Keys.ToList();
                list.Sort();
                foreach (var item in list)
                {
                    query.OrderBy(order[item]);
                }
            }
            else
            {
                var primaryKeyColum = "id";
                Dictionary<int, string> order = new Dictionary<int, string>();

                var primKeyAttri = currentType.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
                if (primKeyAttri.Any())
                    primaryKeyColum = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;

                foreach (var property in currentType.GetProperties())
                {
                    var keyAttri = property.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyColumnAttribute));
                    if (keyAttri.Any())
                        primaryKeyColum = property.Name;

                    var orderAttri = (UIOMaticSortOrderAttribute)property.GetCustomAttributes().FirstOrDefault(x => x.GetType() == typeof(UIOMaticSortOrderAttribute));
                    if (orderAttri != null)
                    {
                        var columnAttri =
                               property.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                        var propName = property.Name;
                        if (columnAttri.Any())
                            propName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;
                        if (string.IsNullOrWhiteSpace(propName))
                        {
                            propName = property.Name;
                        }
                        if (propName.IndexOf("[") < 0)
                        {
                            propName = "([" + propName + "])";
                        }
                        if (orderAttri.IsDescending)
                        {
                            propName += " desc";
                        }
                        order.Add(orderAttri.Sequence, propName);
                    }
                }
                if (order.Count > 0)
                {
                    var list = order.Keys.ToList();
                    list.Sort();
                    foreach (var item in list)
                    {
                        query.OrderBy(order[item]);
                    }
                }
                else
                {
                    query.OrderBy(primaryKeyColum + " asc");
                }

            }

            EventHandler<QueryEventArgs> temp = BuildedQuery;
            if (temp != null)
                temp(this, new QueryEventArgs(tableName.Value, query));
            var sql = query.ToString();
            try
            {
                var p = db.Page<dynamic>(queryinfo.PageNumber, queryinfo.ItemsPerPage, query);
                var result = new UIOMaticPagedResult
                {
                    CurrentPage = p.CurrentPage,
                    ItemsPerPage = p.ItemsPerPage,
                    TotalItems = p.TotalItems,
                    TotalPages = p.TotalPages
                };
                var items = new List<object>();

                foreach (dynamic item in p.Items)
                {
                    // get settable public properties of the type
                    var props = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(x => x.GetSetMethod() != null);

                    // create an instance of the type
                    var obj = Activator.CreateInstance(currentType);


                    // set property values using reflection
                    var values = (IDictionary<string, object>)item;
                    foreach (var prop in props)
                    {
                        //UIOMaticIgnoreFromListViewAttribute
                        var IsIgnoreFromListView = prop.GetCustomAttributes().Where(x => x.GetType() == typeof(IgnoreAttribute));
                        if (IsIgnoreFromListView.Any())
                        {
                            continue;
                        }
                        var columnAttri =
                               prop.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));

                        var propName = prop.Name;
                        if (columnAttri.Any())
                            propName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;
                        if (string.IsNullOrWhiteSpace(propName))
                        {
                            propName = prop.Name;
                        }
                        prop.SetValue(obj, values[propName]);
                    }

                    items.Add(obj);
                }
                result.Items = items;
                return result;
            }
            catch (Exception ex)
            {

                return null;
            }


        }
    }
}