using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIOMatic.Extensions;
using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Persistence;
using System.ComponentModel.DataAnnotations;
using NPoco;
using UIOMatic.ContentApps;
using Umbraco.Core.Composing;

namespace UIOMatic.Services
{
    
    public class NPocoObjectService : IUIOMaticObjectService

        
    {
    

        public IEnumerable<object> GetAll(Type type, string sortColumn = "", string sortOrder = "")
        {
            var typeInfo = GetTypeInfo(type); 
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var repo = Helper.GetRepository(attri, typeInfo);

            return repo.GetAll(sortColumn, sortOrder);
        }

        public UIOMaticPagedResult GetPaged(Type type, int itemsPerPage, int pageNumber, 
            string sortColumn = "", string sortOrder = "",
            IDictionary<string, string> filters = null,
            string searchTerm = "")
        {
            var typeInfo = GetTypeInfo(type);
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var repo = Helper.GetRepository(attri, typeInfo);

            return repo.GetPaged(pageNumber, itemsPerPage, searchTerm, filters, sortColumn, sortOrder);
        }
        public UIOMaticPagedResult GetPagedWithNodeId(Type type, int nodeId, string nodeIdField, int itemsPerPage, int pageNumber,
          string sortColumn, string sortOrder, IDictionary<string, string> filters, string searchTerm)
        {
            var typeInfo = GetTypeInfo(type);
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var repo = Helper.GetRepository(attri, typeInfo);

            filters.Add(nodeIdField,nodeId.ToString());

            return repo.GetPaged(pageNumber, itemsPerPage, searchTerm, filters, sortColumn, sortOrder);
        }

        public object GetById(Type type, string id)
        {
            var typeInfo = GetTypeInfo(type);
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var repo = Helper.GetRepository(attri, typeInfo);            

            return repo.Get(id);
        }

        public object Create(Type type, IDictionary<string, object> values)
        {
            var obj = CreateAndPopulateType(type, values);

            var typeInfo = GetTypeInfo(type);
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var repo = Helper.GetRepository(attri, typeInfo);
            
            var a1 = new ObjectEventArgs(typeInfo.Type, obj);
            UIOMaticObjectService.OnCreatingObject(a1);
            obj = a1.Object;

            obj = repo.Create(obj);

            var a2 = new ObjectEventArgs(typeInfo.Type, obj);
            UIOMaticObjectService.OnCreatedObject(a2);

            return a2.Object;
        }

        public object Update(Type type, IDictionary<string, object> values)
        {
            var obj = CreateAndPopulateType(type, values);

            var typeInfo = GetTypeInfo(type);
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var repo = Helper.GetRepository(attri, typeInfo);

            var a1 = new ObjectEventArgs(typeInfo.Type, obj);
            UIOMaticObjectService.OnUpdatingObject(a1);
            obj = a1.Object;

            obj = repo.Update(obj);

            var a2 = new ObjectEventArgs(typeInfo.Type, obj);
            UIOMaticObjectService.OnUpdatedObject(a2);

            return a2.Object;
        }

        public string[] DeleteByIds(Type type, string[] ids)
        {
            var typeInfo = GetTypeInfo(type);
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var repo = Helper.GetRepository(attri, typeInfo);
            
            var a1 = new DeleteEventArgs(typeInfo.Type, ids);
            UIOMaticObjectService.OnDeletingObjects(a1);
            ids = a1.Ids;

            repo.Delete(ids);
            
            var a2 = new DeleteEventArgs(typeInfo.Type, ids);
            UIOMaticObjectService.OnDeletedObjects(a2);

            return a2.Ids;
        }

        public long GetTotalRecordCount(Type type)
        {
            var typeInfo = GetTypeInfo(type);
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            var repo = Helper.GetRepository(attri, typeInfo);

            return repo.GetTotalRecordCount();
        }

        //TODO: Move validation out of ObjectService? as I think it isn't PetaPoco specific
        public IEnumerable<ValidationResult> Validate(Type type, IDictionary<string, object> values)
        {
            var obj = CreateAndPopulateType(type, values);

            var context = new ValidationContext(obj, null, null);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(obj, context, results, true);

            return results;
        }

        public IEnumerable<string> GetAllColumns(Type type)
        {
            foreach (var prop in type.GetProperties())
            {
                var attri = prop.GetCustomAttribute<IgnoreAttribute>();
                if (attri == null)
                {
                    var column = prop.GetColumnName();
                    if (!column.IsNullOrWhiteSpace())
                        yield return prop.GetColumnName();
                }
            }
        }

        public IEnumerable<object> GetFilterLookup(Type type, string keyPropertyName, string valuePropertyName)
        {
            // Sorry Marc, I don't actually think this is the best way of doing this as we are fetching all objects
            // and getting the distinct values in memory, which could be quite intensive, 
            // but I can't think of another way right now.

            var distinctData = new Dictionary<string, object>(); 
            var data = GetAll(type); 

            foreach (var dataItem in data)
            {
                var keyPropValue = type.GetPropertyValue(keyPropertyName, dataItem);
                if (keyPropValue != null && !distinctData.ContainsKey(keyPropValue.ToString()))
                {
                    distinctData.Add(keyPropValue.ToString(), dataItem);
                }
            }

            var returnData = distinctData.Values.Select(x => new {
                key = type.GetPropertyValue(keyPropertyName, x),
                value = type.GetPropertyValue(valuePropertyName, x)
            });

            return returnData;
        }

        public UIOMaticTypeInfo GetTypeInfo(Type type, bool populateProperties =  false)
        {
          
            // Types shouldn't change without an app pool recycle so might as well cache these
            return (UIOMaticTypeInfo)Umbraco.Web.Composing.Current.AppCaches.RuntimeCache.Get("PetaPocoObjectService_GetTypeInfo_" + type.AssemblyQualifiedName + "_" + populateProperties, () =>
            {
                var attri = type.GetCustomAttribute<UIOMaticAttribute>();

                var editableProperties = new List<UIOMaticEditablePropertyInfo>();
                var listViewProperties = new List<UIOMaticViewablePropertyInfo>();
                var listViewFilterProperties = new List<UIOMaticFilterPropertyInfo>();
                var rawProperties = new List<UIOMaticPropertyInfo>();
                var actions = new List<UIOMaticActionInfo>();

                var nameFieldKey = "";
                var dateCreatedFieldKey = "";
                var dateModifiedFieldKey = "";

                var props = type.GetProperties().ToArray();
                foreach (var prop in props)
                {
                    var attris = prop.GetCustomAttributes().ToArray();

                    // Get date created property key
                    if (attris.Any(x => x.GetType() == typeof(UIOMaticDateCreatedAttribute)))
                    {
                        dateCreatedFieldKey = prop.Name;
                    }

                    // Get date modified property key
                    if (attris.Any(x => x.GetType() == typeof(UIOMaticDateModifiedAttribute)))
                    {
                        dateModifiedFieldKey = prop.Name;
                    }

                    // Process properties
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
                                if (prop.PropertyType == typeof(bool)) view = Constants.FieldEditors.ViewPaths[Constants.FieldEditors.CheckBox];
                                if (prop.PropertyType == typeof(DateTime)) view = Constants.FieldEditors.ViewPaths[Constants.FieldEditors.DateTime];
                                if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long)) view = Constants.FieldEditors.ViewPaths[Constants.FieldEditors.Number];
                            }

                            var pi = new UIOMaticEditablePropertyInfo
                            {
                                Key = prop.Name,
                                Name = attri2.Name.IsNullOrWhiteSpace() ? prop.Name.ToSentenceCase() : attri2.Name,
                                ColumnName = prop.GetColumnName(),
                                Tab = attri2.Tab.IsNullOrWhiteSpace() ? "General" : attri2.Tab,
                                TabOrder = attri2.TabOrder,
                                Description = attri2.Description,
                                View = IOHelper.ResolveUrl(view),
                                Type = prop.PropertyType.ToString(),
                                Config = attri2.Config.IsNullOrWhiteSpace() ? null : (JObject)JsonConvert.DeserializeObject(attri2.Config),
                                Order = attri2.Order
                            };


                            if (attri2.IsNameField)
                            {
                                nameFieldKey = prop.Name;
                            }

                            editableProperties.Add(pi);
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

                            var pi = new UIOMaticViewablePropertyInfo
                            {
                                Key = prop.Name,
                                Name = attri3.Name.IsNullOrWhiteSpace() ? prop.Name : attri3.Name,
                                ColumnName = prop.GetColumnName(),
                                View = IOHelper.ResolveUrl(view),
                                Type = prop.PropertyType.ToString(),
                                Config = attri3.Config.IsNullOrWhiteSpace() ? null : (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(attri3.Config),
                                Order = attri3.Order
                            };

                            listViewProperties.Add(pi);
                        }

                        // Check for list view filter properties
                        var attri4 = attris.FirstOrDefault(x => x.GetType() == typeof(UIOMaticListViewFilterAttribute)) as UIOMaticListViewFilterAttribute;
                        if (attri4 != null)
                        {
                            var view = attri4.GetView();
                            var keyProp = attri4.KeyField.IsNullOrWhiteSpace() ? prop : props.FirstOrDefault(x => x.Name == attri4.KeyField);

                            // Handle custom views?
                            var pi = new UIOMaticFilterPropertyInfo
                            {
                                Key = prop.Name,
                                Name = attri4.Name.IsNullOrWhiteSpace() ? prop.Name.ToSentenceCase() : attri4.Name,
                                ColumnName = prop.GetColumnName(),
                                KeyPropertyName = keyProp.Name,
                                KeyColumnName = keyProp.GetColumnName(),
                                View = IOHelper.ResolveUrl(view),
                                Type = prop.PropertyType.ToString(),
                                Config = attri4.Config.IsNullOrWhiteSpace() ? null : (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(attri4.Config),
                                Order = attri4.Order
                            };

                            listViewFilterProperties.Add(pi);
                        }

                        // Check for date/modified properties
                        if (prop.GetCustomAttribute<UIOMaticDateCreatedAttribute>() != null)
                        {
                            dateCreatedFieldKey = prop.Name;
                        }
                        if (prop.GetCustomAttribute<UIOMaticDateModifiedAttribute>() != null)
                        {
                            dateModifiedFieldKey = prop.Name;
                        }

                        // Raw properties
                        rawProperties.Add(new UIOMaticPropertyInfo
                        {
                            Key = prop.Name,
                            Name = prop.Name,
                            Type = prop.PropertyType.ToString()
                        });
                    }
                }

                // Calculate the types path
                var path = new List<string>(new[] { attri.Alias, attri.ParentAlias });
                var parentTypeAlias = attri.ParentAlias;
                while (parentTypeAlias != "-1")
                {
                    var parentType = Helper.GetUIOMaticTypeByAlias(parentTypeAlias, includeFolders: true);
                    if (parentType != null)
                    {
                        var parentAttri = parentType.GetCustomAttribute<UIOMaticFolderAttribute>();
                        parentTypeAlias = parentAttri.ParentAlias;
                    }
                    else
                    {
                        parentTypeAlias = "-1";
                    }

                    path.Add(parentTypeAlias);
                }
                path.Reverse();

                if(attri.ListViewActions != null)
                {
                    foreach(var action in attri.ListViewActions)
                    {
                        var attri5 = action.GetCustomAttribute<UIOMaticActionAttribute>();

                        if (attri5 != null)
                        {

                            actions.Add(new UIOMaticActionInfo
                            {
                                Alias = attri5.Alias,
                                Name = attri5.Name,
                                View = IOHelper.ResolveUrl(attri5.View),
                                Icon = attri5.Icon,
                                Config = attri5.Config.IsNullOrWhiteSpace() ? null : (JObject)JsonConvert.DeserializeObject(attri5.Config),

                            });
                        }
                    }
                }

                return new UIOMaticTypeInfo
                {
                    Alias = attri.Alias,
                    DisplayNamePlural = attri.FolderName,
                    DisplayNameSingular = attri.ItemName,
                    FolderIcon = attri.FolderIcon,
                    ItemIcon = attri.ItemIcon,
                    Name = type.Name,
                    TableName = type.GetTableName(),
                    RenderType = attri.RenderType,
                    PrimaryKeyColumnName = type.GetPrimaryKeyName(),
                    AutoIncrementPrimaryKey = type.AutoIncrementPrimaryKey(),
                    NameFieldKey = nameFieldKey,
                    ReadOnly = attri.ReadOnly,
                    EditableProperties = editableProperties.OrderBy(x => x.Order).ThenBy(x => x.Name).ToArray(),
                    ListViewProperties = listViewProperties.OrderBy(x => x.Order).ThenBy(x => x.Name).ToArray(),
                    ListViewFilterProperties = listViewFilterProperties.OrderBy(x => x.Order).ThenBy(x => x.Name).ToArray(),
                    RawProperties = rawProperties.ToArray(),
                    Path = path.ToArray(),
                    Type = type,
                    ListViewActions = actions.ToArray(),
                    SortColumn = attri.SortColumn,
                    SortOrder = attri.SortOrder,
                    DateCreatedFieldKey = dateCreatedFieldKey,
                    DateModifiedFieldKey = dateModifiedFieldKey
                };
            });
        }

        public object GetScaffold(Type type)
        {
            var obj = Activator.CreateInstance(type);

            var a1 = new ObjectEventArgs(type, obj);
            UIOMaticObjectService.OnScaffoldingObject(a1);

            return a1.Object;
        }

        private object CreateAndPopulateType(Type type, IDictionary<string, object> values)
        {
            var settings =  new JsonSerializerSettings
            {
                ContractResolver = new UIOMatic.Serialization.UIOMaticSerializerContractResolver()
            };

            var json = JsonConvert.SerializeObject(values, settings);
            var obj = JsonConvert.DeserializeObject(json, type,settings);
            return obj;
        }

      
    }
}
