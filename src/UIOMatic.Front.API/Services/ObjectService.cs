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
using System.ComponentModel.DataAnnotations;
using UIOMatic.Services;
using SqlKata;
using UIOMatic.Front.API.Extensions;
using UIOMatic.Front.API;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace UIOMatic.Front.API.Services
{
    public class ObjectService : IUIOMaticObjectService
    {
        private readonly IUIOMaticHelper _helper;
        private readonly IUIOMaticRepositoryFactory _repositoryFactory;
        private readonly UIOMaticObjectService _uioMaticObjectService;
        private readonly IMemoryCache _memCache;

        public ObjectService(
            IUIOMaticHelper helper,
            IUIOMaticRepositoryFactory repositoryFactory,
            UIOMaticObjectService uioMaticObjectService,
            IMemoryCache memCache)
        {
            _helper = helper;
            _repositoryFactory = repositoryFactory;
            _uioMaticObjectService = uioMaticObjectService;
            _memCache = memCache;
        }

        public IEnumerable<UIOMaticPropertyInfo> GetPropertyEditors(Type type)
        {
            var typeInfo = GetTypeInfo(type);
            return typeInfo.Properties.Where(x => x.Attribute.ShowInListView);
        }

        public IEnumerable<UIOMaticPropertyInfo> GetFields(Type type)
        {
            var typeInfo = GetTypeInfo(type);
            return typeInfo.Properties;
        }

        public IEnumerable<object> GetAllUsers()
        {
            // This is a placeholder - in a real implementation, you would need to integrate with your user system
            return new List<object>();
        }

        public UIOMaticTypeInfo GetTypeInfo(Type type, bool populateProperties = false)
        {
            return _uioMaticObjectService.GetTypeInfo(type, populateProperties);
        }

        public object GetScaffold(Type type)
        {
            var obj = Activator.CreateInstance(type);
            var a1 = new ObjectEventArgs(type, obj);
            _uioMaticObjectService.OnScaffoldingObject(a1);
            return a1.Object;
        }

        public object CreateAndPopulateType(Type type, IDictionary<string, object> values)
        {
            return MapToObject(type, values);
        }

        public object MapToObject(Type type, IDictionary<string, object> values)
        {
            var obj = Activator.CreateInstance(type);
            var typeInfo = GetTypeInfo(type);

            foreach (var prop in typeInfo.Properties)
            {
                if (values.ContainsKey(prop.Name))
                {
                    var value = values[prop.Name];
                    if (value != null)
                    {
                        prop.SetValue(obj, value);
                    }
                }
            }

            return obj;
        }

        public IDictionary<string, object> MapToDocument(object obj)
        {
            var typeInfo = GetTypeInfo(obj.GetType());
            var result = new Dictionary<string, object>();

            foreach (var prop in typeInfo.Properties)
            {
                var value = prop.GetValue(obj);
                if (value != null)
                {
                    result[prop.Name] = value;
                }
            }

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
            return typeInfo.Properties.Select(x => x.Name);
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

        public async Task<object> GetByIdAsync(Type type, string id)
        {
            var repository = _repositoryFactory.GetRepository(type);
            return await repository.GetAsync(id);
        }

        public async Task<object> CreateAsync(Type type, IDictionary<string, object> values)
        {
            var repository = _repositoryFactory.GetRepository(type);
            var entity = MapToObject(type, values);
            return await repository.CreateAsync(entity);
        }

        public async Task<object> UpdateAsync(Type type, IDictionary<string, object> values)
        {
            var repository = _repositoryFactory.GetRepository(type);
            var entity = MapToObject(type, values);
            return await repository.UpdateAsync(entity);
        }

        public async Task<string[]> DeleteByIdsAsync(Type type, string[] ids)
        {
            var repository = _repositoryFactory.GetRepository(type);
            await repository.DeleteAsync(ids);
            return ids;
        }

        public async Task<long> GetTotalRecordCountAsync(Type type)
        {
            var repository = _repositoryFactory.GetRepository(type);
            return await repository.GetTotalRecordCountAsync();
        }

        public async Task<IEnumerable<object>> GetAllAsync(Type type, string sortColumn, string sortOrder)
        {
            var repository = _repositoryFactory.GetRepository(type);
            return await repository.GetAllAsync(sortColumn, sortOrder);
        }

        public async Task<UIOMaticPagedResult> GetPagedAsync(Type type, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, IDictionary<string, string> filters, string searchTerm)
        {
            var repository = _repositoryFactory.GetRepository(type);
            return await repository.GetPagedAsync(pageNumber, itemsPerPage, searchTerm, filters, sortColumn, sortOrder);
        }

        public async Task<UIOMaticPagedResult> GetPagedWithNodeIdAsync(Type type, int nodeId, string nodeIdField, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, IDictionary<string, string> filters, string searchTerm)
        {
            var repository = _repositoryFactory.GetRepository(type);
            filters ??= new Dictionary<string, string>();
            filters[nodeIdField] = nodeId.ToString();
            return await repository.GetPagedAsync(pageNumber, itemsPerPage, searchTerm, filters, sortColumn, sortOrder);
        }

        public async Task<IEnumerable<object>> GetFilterLookupAsync(Type type, string keyPropertyName, string valuePropertyName)
        {
            var repository = _repositoryFactory.GetRepository(type);
            var items = await repository.GetAllAsync();
            return items.Select(x =>
            {
                var key = x.GetType().GetProperty(keyPropertyName)?.GetValue(x);
                var value = x.GetType().GetProperty(valuePropertyName)?.GetValue(x);
                return new { Key = key, Value = value };
            });
        }
    }
}
