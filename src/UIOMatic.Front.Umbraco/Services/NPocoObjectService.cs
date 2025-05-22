using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UIOMatic.Extensions;
using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using NPoco;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Hosting;
using UIOMatic.Services;
using UIOMatic.Front.Umbraco.Extensions;
using System.Reflection;
using Umbraco.Cms.Infrastructure.Scoping;
using IHostingEnvironment = Umbraco.Cms.Core.Hosting.IHostingEnvironment;
using Umbraco.Cms.Core.Services;

namespace UIOMatic.Front.Umbraco.Services
{
    public class NPocoObjectService : IUIOMaticObjectService
    {
        private readonly AppCaches _appCaches;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUIOMaticHelper _helper;
        private readonly UIOMaticObjectService _uioMaticObjectService;
        private readonly IScopeProvider _scopeProvider;
        private readonly IUserService _userService;

        public NPocoObjectService(
            AppCaches appCaches,
            IHostingEnvironment hostingEnvironment,
            IUIOMaticHelper helper,
            UIOMaticObjectService uioMaticObjectService,
            IScopeProvider scopeProvider,
            IUserService userService)
        {
            _appCaches = appCaches;
            _hostingEnvironment = hostingEnvironment;
            _helper = helper;
            _uioMaticObjectService = uioMaticObjectService;
            _scopeProvider = scopeProvider;
            _userService = userService;
        }

        public async Task<IEnumerable<object>> GetAllAsync(Type type)
        {
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null)
                throw new InvalidOperationException($"Type {type.Name} does not have a UIOMaticAttribute");

            var typeInfo = GetTypeInfo(type);
            var repository = _helper.GetRepository(attri, typeInfo);
            using var scope = _scopeProvider.CreateScope();
            var result = await repository.GetAllAsync();
            scope.Complete();
            return result;
        }

        public async Task<IEnumerable<object>> GetPagedAsync(Type type, int pageNumber, int pageSize, string sortColumn, string sortOrder, string searchTerm)
        {
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null)
                throw new InvalidOperationException($"Type {type.Name} does not have a UIOMaticAttribute");

            var typeInfo = GetTypeInfo(type);
            var repository = _helper.GetRepository(attri, typeInfo);
            using var scope = _scopeProvider.CreateScope();
            var filters = new Dictionary<string, string>();
            var result = await repository.GetPagedAsync(pageNumber, pageSize, searchTerm, filters, sortColumn, sortOrder);
            scope.Complete();
            return result.Items;
        }

        public async Task<object> GetByIdAsync(Type type, string id)
        {
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null)
                throw new InvalidOperationException($"Type {type.Name} does not have a UIOMaticAttribute");
            var typeInfo = GetTypeInfo(type);
            var repository = _helper.GetRepository(attri, typeInfo);
            using var scope = _scopeProvider.CreateScope();
            var result = await repository.GetAsync(id);
            scope.Complete();
            return result;
        }

        public async Task<object> CreateAsync(Type type, IDictionary<string, object> values)
        {
            var entity = MapToObject(type, values);
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null)
                throw new InvalidOperationException($"Type {type.Name} does not have a UIOMaticAttribute");
            var typeInfo = GetTypeInfo(type);
            var repository = _helper.GetRepository(attri, typeInfo);
            using var scope = _scopeProvider.CreateScope();
            var result = await repository.CreateAsync(entity);
            scope.Complete();
            return result;
        }

        public async Task<object> UpdateAsync(Type type, IDictionary<string, object> values)
        {
            var entity = MapToObject(type, values);
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null)
                throw new InvalidOperationException($"Type {type.Name} does not have a UIOMaticAttribute");
            var typeInfo = GetTypeInfo(type);
            var repository = _helper.GetRepository(attri, typeInfo);
            using var scope = _scopeProvider.CreateScope();
            var result = await repository.UpdateAsync(entity);
            scope.Complete();
            return result;
        }

        public async Task<string[]> DeleteByIdsAsync(Type type, string[] ids)
        {
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null)
                throw new InvalidOperationException($"Type {type.Name} does not have a UIOMaticAttribute");
            var typeInfo = GetTypeInfo(type);
            var repository = _helper.GetRepository(attri, typeInfo);
            using var scope = _scopeProvider.CreateScope();
            await repository.DeleteAsync(ids);
            scope.Complete();
            return ids;
        }

        public async Task<IEnumerable<object>> GetAllAsync(Type type, string sortColumn, string sortOrder)
        {
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null)
                throw new InvalidOperationException($"Type {type.Name} does not have a UIOMaticAttribute");
            var typeInfo = GetTypeInfo(type);
            var repository = _helper.GetRepository(attri, typeInfo);
            using var scope = _scopeProvider.CreateScope();
            var result = await repository.GetAllAsync(sortColumn, sortOrder);
            scope.Complete();
            return result;
        }

        public async Task<UIOMaticPagedResult> GetPagedAsync(Type type, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, IDictionary<string, string> filters, string searchTerm)
        {
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null)
                throw new InvalidOperationException($"Type {type.Name} does not have a UIOMaticAttribute");
            var typeInfo = GetTypeInfo(type);
            var repository = _helper.GetRepository(attri, typeInfo);
            using var scope = _scopeProvider.CreateScope();
            var result = await repository.GetPagedAsync(pageNumber, itemsPerPage, searchTerm, filters, sortColumn, sortOrder);
            scope.Complete();
            return result;
        }

        public async Task<UIOMaticPagedResult> GetPagedWithNodeIdAsync(
            Type type,
            int nodeId,
            string nodeIdField,
            int itemsPerPage,
            int pageNumber,
            string sortColumn,
            string sortOrder,
            IDictionary<string, string> filters,
            string searchTerm)
        {
            var typeInfo = GetTypeInfo(type);
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null)
            {
                throw new InvalidOperationException($"Type {type.Name} is not decorated with UIOMaticAttribute");
            }
            var repo = _helper.GetRepository(attri, typeInfo);

            filters ??= new Dictionary<string, string>();
            filters[nodeIdField] = nodeId.ToString();

            using var scope = _scopeProvider.CreateScope();
            var result = await repo.GetPagedAsync(pageNumber, itemsPerPage, searchTerm, filters, sortColumn, sortOrder);
            scope.Complete();
            return result;
        }

        public async Task<long> GetTotalRecordCountAsync(Type type)
        {
            var typeInfo = GetTypeInfo(type);
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null)
            {
                throw new InvalidOperationException($"Type {type.Name} is not decorated with UIOMaticAttribute");
            }
            var repo = _helper.GetRepository(attri, typeInfo);

            using var scope = _scopeProvider.CreateScope();
            var result = await repo.GetTotalRecordCountAsync();
            scope.Complete();
            return result;
        }

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
            var typeInfo = GetTypeInfo(type);
            return typeInfo.RawProperties.Select(x => x.Name);
        }

        public async Task<IEnumerable<object>> GetFilterLookupAsync(Type type, string keyPropertyName, string valuePropertyName)
        {
            var typeInfo = GetTypeInfo(type);
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null)
            {
                throw new InvalidOperationException($"Type {type.Name} is not decorated with UIOMaticAttribute");
            }
            var repo = _helper.GetRepository(attri, typeInfo);

            using var scope = _scopeProvider.CreateScope();
            var result = await repo.GetAllAsync();
            scope.Complete();

            return result.Select(x =>
            {
                var key = x.GetType().GetProperty(keyPropertyName)?.GetValue(x);
                var value = x.GetType().GetProperty(valuePropertyName)?.GetValue(x);

                return new { Key = key, Value = value };
            });
        }

        public UIOMaticTypeInfo GetTypeInfo(Type type, bool populateProperties = false)
        {
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null) return null;

            var typeInfo = new UIOMaticTypeInfo
            {
                Alias = attri.Alias,
                DisplayNamePlural = attri.FolderName,
                DisplayNameSingular = attri.ItemName,
                FolderIcon = attri.FolderIcon,
                ItemIcon = attri.ItemIcon,
                Name = type.Name,
                TableName = type.Name,
                PrimaryKeyColumnName = "Id",
                AutoIncrementPrimaryKey = true,
                RenderType = attri.RenderType,
                ReadOnly = attri.ReadOnly,
                Path = new[] { attri.FolderName },
                Type = type,
                SortColumn = attri.SortColumn,
                SortOrder = attri.SortOrder
            };

            if (populateProperties)
            {
                typeInfo.RawProperties = GetPropertyEditors(type).ToArray();
                typeInfo.EditableProperties = GetFields(type).OfType<UIOMaticEditablePropertyInfo>().ToArray();
                typeInfo.ListViewProperties = GetFields(type).OfType<UIOMaticViewablePropertyInfo>().ToArray();
                typeInfo.ListViewFilterProperties = GetFields(type).OfType<UIOMaticFilterPropertyInfo>().ToArray();
            }

            return typeInfo;
        }

        public object GetScaffold(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public IEnumerable<object> GetAllUsers()
        {
            return _userService.GetAll(0, int.MaxValue, out _).Select(x => new { Id = x.Id, Name = x.Name });
        }

        public object CreateAndPopulateType(Type type, IDictionary<string, object> values)
        {
            var entity = Activator.CreateInstance(type);
            foreach (var kvp in values)
            {
                entity.SetPropertyValue(kvp.Key, kvp.Value);
            }
            return entity;
        }

        public object MapToObject(Type type, IDictionary<string, object> values)
        {
            return CreateAndPopulateType(type, values);
        }

        public IDictionary<string, object> MapToDocument(object obj)
        {
            var result = new Dictionary<string, object>();
            var properties = obj.GetType().GetProperties();
            foreach (var prop in properties)
            {
                result[prop.Name] = prop.GetValue(obj);
            }
            return result;
        }

        public void OnCreatingObject(ObjectEventArgs args)
        {
            _uioMaticObjectService.OnCreatingObject(args);
        }

        public void OnCreatedObject(ObjectEventArgs args)
        {
            _uioMaticObjectService.OnCreatedObject(args);
        }

        public void OnUpdatingObject(ObjectEventArgs args)
        {
            _uioMaticObjectService.OnUpdatingObject(args);
        }

        public void OnUpdatedObject(ObjectEventArgs args)
        {
            _uioMaticObjectService.OnUpdatedObject(args);
        }

        public void OnDeletingObjects(DeleteEventArgs args)
        {
            _uioMaticObjectService.OnDeletingObjects(args);
        }

        public void OnDeletedObjects(DeleteEventArgs args)
        {
            _uioMaticObjectService.OnDeletedObjects(args);
        }

        public void OnScaffoldingObject(ObjectEventArgs args)
        {
            _uioMaticObjectService.OnScaffoldingObject(args);
        }

        public void OnBuildingQuery(QueryEventArgs args)
        {
            _uioMaticObjectService.OnBuildingQuery(args);
        }

        public void OnBuiltQuery(QueryEventArgs args)
        {
            _uioMaticObjectService.OnBuiltQuery(args);
        }

        private UIOMaticTypeInfo BuildTypeInfo(Type type, bool populateProperties)
        {
            var properties = type.GetProperties();
            var propertyList = new List<UIOMaticPropertyInfo>();

            foreach (var prop in properties)
            {
                var attri = prop.GetCustomAttribute<UIOMaticFieldAttribute>();
                if (attri != null)
                {
                    var propertyInfo = new UIOMaticPropertyInfo
                    {
                        Name = prop.Name,
                        Key = prop.Name,
                        ColumnName = prop.Name,
                        Type = prop.PropertyType.Name,
                        Order = attri.Order
                    };

                    propertyList.Add(propertyInfo);
                }
            }

            var typeInfo = new UIOMaticTypeInfo
            {
                Type = type,
                RawProperties = propertyList.ToArray()
            };

            if (populateProperties)
            {
                typeInfo.EditableProperties = propertyList.OfType<UIOMaticEditablePropertyInfo>().ToArray();
                typeInfo.ListViewProperties = propertyList.OfType<UIOMaticViewablePropertyInfo>().ToArray();
                typeInfo.ListViewFilterProperties = propertyList.OfType<UIOMaticFilterPropertyInfo>().ToArray();
            }

            return typeInfo;
        }

        public IEnumerable<UIOMaticPropertyInfo> GetPropertyEditors(Type type)
        {
            var properties = type.GetProperties();
            var result = new List<UIOMaticPropertyInfo>();

            foreach (var prop in properties)
            {
                var fieldAttri = prop.GetCustomAttribute<UIOMaticFieldAttribute>();
                if (fieldAttri == null) continue;

                var property = new UIOMaticPropertyInfo
                {
                    Key = prop.Name,
                    Name = fieldAttri.Name,
                    ColumnName = prop.Name,
                    Type = prop.PropertyType.Name,
                    Order = fieldAttri.Order
                };

                result.Add(property);
            }

            return result;
        }

        public IEnumerable<UIOMaticPropertyInfo> GetFields(Type type)
        {
            var properties = type.GetProperties();
            var result = new List<UIOMaticPropertyInfo>();

            foreach (var prop in properties)
            {
                var fieldAttri = prop.GetCustomAttribute<UIOMaticFieldAttribute>();
                if (fieldAttri == null) continue;

                var property = new UIOMaticPropertyInfo
                {
                    Key = prop.Name,
                    Name = fieldAttri.Name,
                    ColumnName = prop.Name,
                    Type = prop.PropertyType.Name,
                    Order = fieldAttri.Order
                };

                result.Add(property);
            }

            return result;
        }
    }
}




