using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using UIOMatic.Attributes;
using UIOMatic.Core.Repositories;
using LiteDB;
using System.IO;
using UIOMatic.Services;
using Umbraco.Cms.Core.Scoping;

namespace UIOMatic.Front.API.Repositories
{
    public class LiteDBRepository : UIOMaticRepositoryBase
    {
        private readonly UIOMaticObjectService _uioMaticObjectService;
        private readonly UIOMaticAttribute _attr;
        private readonly UIOMaticTypeInfo _typeInfo;
        private readonly string _dbPath;
        private string _collectionName;
        private Type _entityType;
        private readonly LiteDatabase _db;

        public LiteDBRepository(
            UIOMaticAttribute attr,
            UIOMaticTypeInfo typeInfo,
            UIOMaticObjectService uioMaticObjectService)
        {
            _attr = attr;
            _typeInfo = typeInfo;
            _uioMaticObjectService = uioMaticObjectService;
            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "UIOMatic", "LiteDB", $"{typeInfo.Type.Name}.db");
            _db = new LiteDatabase(_dbPath);
            _collectionName = typeInfo.Type.Name;
            _entityType = typeInfo.Type;
        }

        public override void Initialize(object[] parameters)
        {
            if (parameters == null || parameters.Length < 2)
                throw new ArgumentException("LiteDBRepository requires collection name and entity type parameters");

            _collectionName = parameters[0] as string;
            _entityType = parameters[1] as Type;

            if (string.IsNullOrEmpty(_collectionName) || _entityType == null)
                throw new ArgumentException("Invalid parameters for LiteDBRepository");

            // Ensure the collection exists
            if (!_db.CollectionExists(_collectionName))
            {
                _db.GetCollection(_collectionName);
            }
        }

        public override IEnumerable<object> GetAll(string sortColumn, string sortOrder)
        {
            var collection = _db.GetCollection(_collectionName);
            var query = collection.Query();

            // Apply sorting if specified
            if (!string.IsNullOrEmpty(sortColumn))
            {
                query = sortOrder?.ToLower() == "desc" 
                    ? query.OrderByDescending(sortColumn)
                    : query.OrderBy(sortColumn);
            }

            return query.ToEnumerable();
        }

        public override object GetById(string id)
        {
            var collection = _db.GetCollection(_collectionName);
            return collection.FindById(new ObjectId(id));
        }

        public override object Create(object entity)
        {
            var collection = _db.GetCollection(_collectionName);
            var doc = BsonMapper.Global.ToDocument(entity);
            var id = collection.Insert(doc);
            var idProperty = _entityType.GetProperty("Id");
            if (idProperty != null)
            {
                idProperty.SetValue(entity, id.ToString());
            }
            return entity;
        }

        public override object Update(object entity)
        {
            var idProperty = _entityType.GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Entity must have an Id property");
            var id = idProperty.GetValue(entity)?.ToString();
            if (string.IsNullOrEmpty(id))
                throw new InvalidOperationException("Entity Id cannot be null or empty");
            var collection = _db.GetCollection(_collectionName);
            var doc = BsonMapper.Global.ToDocument(entity);
            collection.Update(new ObjectId(id), doc);
            return entity;
        }

        public override void Delete(string[] ids)
        {
            var collection = _db.GetCollection(_collectionName);
            foreach (var id in ids)
            {
                collection.Delete(new ObjectId(id));
            }
        }

        public override IEnumerable<ValidationResult> Validate(object entity)
        {
            var context = new ValidationContext(entity);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(entity, context, results, true);
            return results;
        }

        public override object Get(string id)
        {
            return GetById(id);
        }

        public override long GetTotalRecordCount()
        {
            var collection = _db.GetCollection(_collectionName);
            return collection.Count();
        }

        public override UIOMaticPagedResult GetPaged(int pageNumber, int itemsPerPage, string searchTerm, IDictionary<string, string> filters, string sortColumn, string sortOrder)
        {
            var collection = _db.GetCollection(_collectionName);
            var query = collection.Query();

            // Apply search if specified
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => 
                    _entityType.GetProperties()
                        .Any(p => p.GetValue(x) != null && 
                            p.GetValue(x).ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            // Apply filters if specified
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    var property = _entityType.GetProperty(filter.Key);
                    if (property != null)
                    {
                        query = query.Where(x => property.GetValue(x) != null && 
                            property.GetValue(x).ToString() == filter.Value);
                    }
                }
            }

            // Apply sorting if specified
            if (!string.IsNullOrEmpty(sortColumn))
            {
                query = sortOrder?.ToLower() == "desc" 
                    ? query.OrderByDescending(sortColumn)
                    : query.OrderBy(sortColumn);
            }

            var totalItems = query.Count();
            var pagedItems = query
                .Skip((pageNumber - 1) * itemsPerPage)
                .Limit(itemsPerPage)
                .ToEnumerable()
                .ToList();

            return new UIOMaticPagedResult
            {
                Items = pagedItems,
                TotalItems = totalItems,
                CurrentPage = pageNumber,
                ItemsPerPage = itemsPerPage,
                TotalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage)
            };
        }

        public void Dispose()
        {
            _db?.Dispose();
        }

        public override async Task<IEnumerable<object>> GetAllAsync(string sortColumn = "", string sortOrder = "")
        {
            var collection = _db.GetCollection(_collectionName);
            var query = collection.Query();

            if (!string.IsNullOrWhiteSpace(_attr.DeletedColumnName))
            {
                query = query.Where(_attr.DeletedColumnName + " = 0");
            }

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query = query.OrderBy(sortColumn, sortOrder == "asc" ? 1 : -1);
            }

            var results = query.ToList();
            return results.Select(x => _uioMaticObjectService.MapToObject(_entityType, x));
        }

        public override async Task<object> GetAsync(string id)
        {
            var collection = _db.GetCollection(_collectionName);
            var result = collection.FindById(new ObjectId(id));
            return result != null ? _uioMaticObjectService.MapToObject(_entityType, result) : null;
        }

        public override async Task<object> CreateAsync(object entity)
        {
            var collection = _db.GetCollection(_collectionName);
            var doc = BsonMapper.Global.ToDocument(entity);
            collection.Insert(doc);
            return entity;
        }

        public override async Task<object> UpdateAsync(object entity)
        {
            var idProperty = _entityType.GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Entity must have an Id property");
            var id = idProperty.GetValue(entity)?.ToString();
            if (string.IsNullOrEmpty(id))
                throw new InvalidOperationException("Entity Id cannot be null or empty");
            var collection = _db.GetCollection(_collectionName);
            var doc = BsonMapper.Global.ToDocument(entity);
            collection.Update(new ObjectId(id), doc);
            return entity;
        }

        public override async Task DeleteAsync(string[] ids)
        {
            var collection = _db.GetCollection(_collectionName);
            foreach (var id in ids)
            {
                collection.Delete(new ObjectId(id));
            }
        }

        public override async Task<object> ValidateAsync(object entity)
        {
            return await Task.FromResult(_uioMaticObjectService.Validate(_entityType, entity));
        }

        public override async Task<long> GetTotalRecordCountAsync()
        {
            var collection = _db.GetCollection(_collectionName);
            return collection.Count();
        }

        public override async Task<UIOMaticPagedResult> GetPagedAsync(
            int pageNumber,
            int itemsPerPage,
            string searchTerm = "",
            IDictionary<string, string> filters = null,
            string sortColumn = "",
            string sortOrder = "")
        {
            var collection = _db.GetCollection(_collectionName);
            var query = collection.Query();

            if (!string.IsNullOrWhiteSpace(_attr.DeletedColumnName))
            {
                query = query.Where(_attr.DeletedColumnName + " = 0");
            }

            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter.Key + " = @0", filter.Value);
                }
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where("$ LIKE @0", "%" + searchTerm + "%");
            }

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query = query.OrderBy(sortColumn, sortOrder == "asc" ? 1 : -1);
            }

            var totalItems = query.Count();
            var results = query.Skip((pageNumber - 1) * itemsPerPage).Limit(itemsPerPage).ToList();

            return new UIOMaticPagedResult
            {
                CurrentPage = pageNumber,
                ItemsPerPage = itemsPerPage,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage),
                Items = results.Select(x => _uioMaticObjectService.MapToObject(_entityType, x))
            };
        }
    }
} 