using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UIOMatic.Extensions;
using UIOMatic.Interfaces;
using UIOMatic.Services;
using UIOMatic.Attributes;
using UIOMatic.Models;
using LiteDB;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace UIOMatic.Front.API.Data
{
    public class LiteDBRepository : IUIOMaticRepository
    {
        private UIOMaticAttribute _config;
        private UIOMaticTypeInfo _typeInfo;
        private readonly UIOMaticObjectService _uioMaticObjectService;
        private readonly string _dbPath;

        public LiteDBRepository(UIOMaticAttribute config,
            UIOMaticTypeInfo typeInfo,
            UIOMaticObjectService uioMaticObjectService,
            string dbPath)
        {
            _config = config;
            _typeInfo = typeInfo;
            _uioMaticObjectService = uioMaticObjectService;
            _dbPath = dbPath;
        }

        public virtual async Task<IEnumerable<object>> GetAllAsync(string sortColumn = null, string sortOrder = null)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection(_typeInfo.TableName);
                var query = collection.Query();

                if (!string.IsNullOrWhiteSpace(_config.DeletedColumnName))
                {
                    query = query.Where(_config.DeletedColumnName + " = 0");
                }

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
                {
                    query = query.OrderBy(sortColumn, sortOrder == "asc" ? 1 : -1);
                }

                return await Task.FromResult(query.ToList());
            }
        }

        public virtual async Task<UIOMaticPagedResult> GetPagedAsync(
            int pageNumber,
            int itemsPerPage,
            string searchTerm = "",
            IDictionary<string, string> filters = null,
            string sortColumn = "",
            string sortOrder = "")
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection(_typeInfo.TableName);
                var query = collection.Query();

                if (!string.IsNullOrWhiteSpace(_config.DeletedColumnName))
                {
                    query = query.Where(_config.DeletedColumnName + " = 0");
                }

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = ApplySearchTerm(query, searchTerm);
                }

                if (filters != null && filters.Any())
                {
                    foreach (var filter in filters)
                    {
                        query = query.Where(filter.Key + " = @0", filter.Value);
                    }
                }

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
                {
                    query = query.OrderBy(sortColumn, sortOrder == "asc" ? 1 : -1);
                }

                var total = await Task.FromResult(query.Count());
                var items = await Task.FromResult(query.Skip((pageNumber - 1) * itemsPerPage).Limit(itemsPerPage).ToList());

                return new UIOMaticPagedResult
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = itemsPerPage,
                    TotalItems = total,
                    TotalPages = (int)Math.Ceiling((double)total / itemsPerPage),
                    Items = items
                };
            }
        }

        public virtual async Task<object> GetAsync(string id)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection(_typeInfo.TableName);
                var query = collection.Query().Where(_typeInfo.PrimaryKeyName + " = @0", id);

                if (!string.IsNullOrWhiteSpace(_config.DeletedColumnName))
                {
                    query = query.Where(_config.DeletedColumnName + " = 0");
                }

                return await Task.FromResult(query.FirstOrDefault());
            }
        }

        public virtual async Task<object> CreateAsync(object entity)
        {
            var a1 = new ObjectEventArgs(_typeInfo.Type, entity);
            _uioMaticObjectService.OnCreatingObject(a1);
            entity = a1.Object;

            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection(_typeInfo.TableName);
                var id = collection.Insert(entity);
                entity = await GetAsync(id.ToString());
            }

            var a2 = new ObjectEventArgs(_typeInfo.Type, entity);
            _uioMaticObjectService.OnCreatedObject(a2);

            return a2.Object;
        }

        public virtual async Task<object> UpdateAsync(object entity)
        {
            var a1 = new ObjectEventArgs(_typeInfo.Type, entity);
            _uioMaticObjectService.OnUpdatingObject(a1);
            entity = a1.Object;

            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection(_typeInfo.TableName);
                collection.Update(entity);
                entity = await GetAsync(entity.GetPropertyValue(_typeInfo.PrimaryKeyName).ToString());
            }

            var a2 = new ObjectEventArgs(_typeInfo.Type, entity);
            _uioMaticObjectService.OnUpdatedObject(a2);

            return a2.Object;
        }

        public virtual async Task DeleteAsync(string[] ids)
        {
            var a1 = new DeleteEventArgs(_typeInfo.Type, ids);
            _uioMaticObjectService.OnDeletingObjects(a1);
            ids = a1.Ids;

            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection(_typeInfo.TableName);
                foreach (var id in ids)
                {
                    if (!string.IsNullOrWhiteSpace(_config.DeletedColumnName))
                    {
                        var entity = await GetAsync(id);
                        if (entity != null)
                        {
                            entity.SetPropertyValue(_config.DeletedColumnName, 1);
                            collection.Update(entity);
                        }
                    }
                    else
                    {
                        collection.Delete(id);
                    }
                }
            }

            var a2 = new DeleteEventArgs(_typeInfo.Type, ids);
            _uioMaticObjectService.OnDeletedObjects(a2);
        }

        public virtual async Task<long> GetTotalRecordCountAsync()
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection(_typeInfo.TableName);
                var query = collection.Query();

                if (!string.IsNullOrWhiteSpace(_config.DeletedColumnName))
                {
                    query = query.Where(_config.DeletedColumnName + " = 0");
                }

                return await Task.FromResult(query.Count());
            }
        }

        private ILiteQueryable<BsonDocument> ApplySearchTerm(ILiteQueryable<BsonDocument> query, string searchTerm)
        {
            var numberDataTypes = new[]
            {
                typeof(Byte), typeof(Decimal), typeof(Double), typeof(Int16), typeof(Int32),
                typeof(Int64), typeof(SByte), typeof(Single), typeof(UInt16), typeof(UInt32),
                typeof(Byte?), typeof(Decimal?), typeof(Double?), typeof(Int16?), typeof(Int32?),
                typeof(Int64?), typeof(SByte?), typeof(Single?), typeof(UInt16?), typeof(UInt32?)
            };

            var guidDataTypes = new[] { typeof(Guid), typeof(Guid?) };
            var dateDataTypes = new[] { typeof(DateTime), typeof(DateTime?) };
            var boolDataTypes = new[] { typeof(bool), typeof(bool?) };

            foreach (var property in _typeInfo.Type.GetProperties())
            {
                var attris = property.GetCustomAttributes(true);
                if (attris.All(x => x.GetType() != typeof(IgnoreAttribute)))
                {
                    var columnName = property.Name;
                    var columnAttri = attris.FirstOrDefault(x => x.GetType() == typeof(ColumnAttribute)) as ColumnAttribute;
                    if (columnAttri != null)
                    {
                        columnName = columnAttri.Name;
                    }

                    if (guidDataTypes.Contains(property.PropertyType))
                    {
                        if (Guid.TryParse(searchTerm, out var searchGuid))
                        {
                            query = query.Where(columnName + " = @0", searchGuid);
                        }
                    }
                    else if (numberDataTypes.Contains(property.PropertyType) || boolDataTypes.Contains(property.PropertyType))
                    {
                        if (decimal.TryParse(searchTerm, out var searchNumber))
                        {
                            query = query.Where(columnName + " = @0", searchNumber);
                        }
                    }
                    else if (dateDataTypes.Contains(property.PropertyType))
                    {
                        if (DateTime.TryParse(searchTerm, out var searchDate))
                        {
                            query = query.Where(columnName + " >= @0 AND " + columnName + " < @1", searchDate.Date, searchDate.AddDays(1).Date);
                        }
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        query = query.Where(columnName + ".Contains(@0)", searchTerm);
                    }
                }
            }

            return query;
        }
    }
} 