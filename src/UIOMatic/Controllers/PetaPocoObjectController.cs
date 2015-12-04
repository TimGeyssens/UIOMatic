﻿using System;
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
                query.OrderBy(strSortColumn + " " + sortOrder);

                if (strSortColumn.IndexOf("[") < 0)
                {
                    strSortColumn = "[" + strSortColumn + "]";
                }
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
                        if (prop.PropertyType == typeof(DateTime) && attri.View == "textfield")
                            view = "~/App_Plugins/UIOMatic/Backoffice/Views/datetime.html";
                        if ((prop.PropertyType == typeof(int) | prop.PropertyType == typeof(long)) && attri.View == "textfield")
                            view = "~/App_Plugins/UIOMatic/Backoffice/Views/number.html";
                        if (!attri.IsCanEdit && isEdit)
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
                            Config = string.IsNullOrEmpty(attri.Config) ? null : (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(attri.Config)
                        };
                        yield return pi;
                    }
                    else
                    {
                        var key = prop.Name;

                        string view = "~/App_Plugins/UIOMatic/Backoffice/Views/textfield.html";
                        if (prop.PropertyType == typeof(bool))
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
            }

            return new UIOMaticTypeInfo()
            {
                RenderType = uioMaticAttri.RenderType,
                PrimaryKeyColumnName = primaryKey,
                IgnoreColumnsFromListView = ignoreColumnsFromListView.ToArray(),
                NameField = nameField,
                DisplayName = uioMaticAttri.Name
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
                //object value
                if (propI.GetCustomAttributes().Any(x => x.GetType() == typeof(UIOMaticIgnoreFieldAttribute)))
                {
                    switch (propI.Name.ToLower())
                    {
                        case "status":
                            Helper.SetValue(ob, propI.Name, "1");
                            continue;
                        case "createddatetime":
                            Helper.SetValue(ob, propI.Name, DateTime.Now);
                            continue;
                        case "createdby":
                            Helper.SetValue(ob, propI.Name, GetCurrentUserId());
                            continue;
                        case "updateddatetime":
                            Helper.SetValue(ob, propI.Name, DateTime.Now);
                            continue;
                        case "updatedby":
                            Helper.SetValue(ob, propI.Name, GetCurrentUserId());
                            continue;
                        //break;
                        default:
                            break;
                    }
                }
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
            ((IUIOMaticModel)ob).SetDefaultValue();
            db.Insert(ob);
            //db.Save(tableName, primaryKeyColum, ob);

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
                if (propI.GetCustomAttributes().Any(x => x.GetType() == typeof(UIOMaticIgnoreFieldAttribute)))
                {
                    switch (propI.Name.ToLower())
                    {
                        case "updateddatetime":
                            Helper.SetValue(ob, propI.Name, DateTime.Now);
                            continue;
                        case "updatedby":
                            Helper.SetValue(ob, propI.Name, GetCurrentUserId());
                            continue;
                        //break;
                        default:
                            break;
                    }
                }
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
            ((IUIOMaticModel)ob).SetDefaultValue();
            db.Save(tableName, primaryKeyColum, ob);
            //db.Save(ob);
            //db.Update(ob);
            return ob;
        }

        public string[] DeleteByIds(string typeOfObject, string ids)
        {
            var currentType = Helper.GetTypesWithUIOMaticAttribute().First(x => x.AssemblyQualifiedName == typeOfObject);
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

            // TODO: Delete with one SQL statement?
            string ids2var = "'" + ids.Replace(",", "','") + "'";
            string DEL_SQL = @"Delete from {0} where {1} in ({2})";
            DEL_SQL = string.Format(DEL_SQL, tableName, primaryKeyColum, ids2var);
            db.Execute(DEL_SQL);
            ////db.Delete()
            //var deletedIds = new List<string>();
            //foreach (var idStr in ids.Split(','))
            //{
            //    var id = "0";
            //    //if (int.TryParse(idStr, out id))
            //    //{
            //        deletedIds.Add(db.Delete(tableName, primaryKeyColum, null, id));
            //    //}
            //}
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
    }
}