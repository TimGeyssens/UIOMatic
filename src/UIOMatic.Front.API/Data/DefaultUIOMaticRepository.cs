using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UIOMatic.Extensions;
using UIOMatic.Interfaces;
using UIOMatic.Services;
using UIOMatic.Attributes;
using UIOMatic.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using SqlKata;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Newtonsoft.Json.Linq;
using static Dapper.SqlMapper;
using UIOMatic.Front.API.Extensions;

namespace UIOMatic.Front.API.Data
{
    public class DefaultUIOMaticRepository : IUIOMaticRepository
    {
        private UIOMaticAttribute _config;
        private UIOMaticTypeInfo _typeInfo;
        private readonly UIOMaticObjectService _uioMaticObjectService;
        private readonly IDbConnection _conn;
        private readonly QueryFactory _db;

        public DefaultUIOMaticRepository(UIOMaticAttribute config,
            UIOMaticTypeInfo typeInfo,
            UIOMaticObjectService uioMaticObjectService,
            QueryFactory db) : this()
        {
            _config = config;
            _typeInfo = typeInfo;
            _uioMaticObjectService = uioMaticObjectService;
            _db = db;
        }

        public DefaultUIOMaticRepository()
        {
        }

        public virtual async Task<IEnumerable<object>> GetAllAsync(string sortColumn = null, string sortOrder = null)
        {
            var q = _db.Query();
            var query = q.Select("*").From(_typeInfo.TableName);
            var result = _db.Compiler.Compile(query);

            var a1 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, result.Sql, sortColumn ?? string.Empty, sortOrder ?? string.Empty, "", null);
            _uioMaticObjectService.OnBuildingQuery(a1);
            query = _db.Query().SelectRaw(a1.Query);

            if (!string.IsNullOrWhiteSpace(_config.DeletedColumnName))
            {
                query.WhereRaw(_config.DeletedColumnName + " = 0");
            }

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query.OrderByRaw(sortColumn + " " + sortOrder);
            }

            result = _db.Compiler.Compile(query);

            var a2 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, result.Sql.Remove(0, 6), sortColumn ?? string.Empty, sortOrder ?? string.Empty, "", null);
            _uioMaticObjectService.OnBuiltQuery(a2);
            return await _db.SelectAsync(a2.Query);
        }

        public virtual async Task<UIOMaticPagedResult> GetPagedAsync(
            int pageNumber,
            int itemsPerPage,
            string searchTerm = "",
            IDictionary<string, string> filters = null,
            string sortColumn = "",
            string sortOrder = "")
        {
            var query = BuildPagedQuery(searchTerm, filters, sortColumn, sortOrder);
            var total = await GetTotalRecordCountAsync();
            var items = await _db.SelectAsync(query);

            return new UIOMaticPagedResult
            {
                CurrentPage = pageNumber,
                ItemsPerPage = itemsPerPage,
                TotalItems = total,
                TotalPages = (int)Math.Ceiling((double)total / itemsPerPage),
                Items = items
            };
        }

        public virtual async Task<object> GetAsync(string id)
        {
            var query = _db.Query()
                .Select("*")
                .From(_typeInfo.TableName)
                .Where(_typeInfo.PrimaryKeyName, id);

            if (!string.IsNullOrWhiteSpace(_config.DeletedColumnName))
            {
                query.WhereRaw(_config.DeletedColumnName + " = 0");
            }

            return await _db.FirstOrDefaultAsync(query);
        }

        public virtual async Task<object> CreateAsync(object entity)
        {
            var a1 = new ObjectEventArgs(_typeInfo.Type, entity);
            _uioMaticObjectService.OnCreatingObject(a1);
            entity = a1.Object;

            var id = await _db.InsertAsync(_typeInfo.TableName, entity);
            entity = await GetAsync(id.ToString());

            var a2 = new ObjectEventArgs(_typeInfo.Type, entity);
            _uioMaticObjectService.OnCreatedObject(a2);

            return a2.Object;
        }

        public virtual async Task<object> UpdateAsync(object entity)
        {
            var a1 = new ObjectEventArgs(_typeInfo.Type, entity);
            _uioMaticObjectService.OnUpdatingObject(a1);
            entity = a1.Object;

            await _db.UpdateAsync(_typeInfo.TableName, entity);
            entity = await GetAsync(entity.GetPropertyValue(_typeInfo.PrimaryKeyName).ToString());

            var a2 = new ObjectEventArgs(_typeInfo.Type, entity);
            _uioMaticObjectService.OnUpdatedObject(a2);

            return a2.Object;
        }

        public virtual async Task DeleteAsync(string[] ids)
        {
            var a1 = new DeleteEventArgs(_typeInfo.Type, ids);
            _uioMaticObjectService.OnDeletingObjects(a1);
            ids = a1.Ids;

            foreach (var id in ids)
            {
                if (!string.IsNullOrWhiteSpace(_config.DeletedColumnName))
                {
                    await _db.Query(_typeInfo.TableName)
                        .Where(_typeInfo.PrimaryKeyName, id)
                        .UpdateAsync(new { [_config.DeletedColumnName] = 1 });
                }
                else
                {
                    await _db.Query(_typeInfo.TableName)
                        .Where(_typeInfo.PrimaryKeyName, id)
                        .DeleteAsync();
                }
            }

            var a2 = new DeleteEventArgs(_typeInfo.Type, ids);
            _uioMaticObjectService.OnDeletedObjects(a2);
        }

        public virtual async Task<long> GetTotalRecordCountAsync()
        {
            var query = _db.Query()
                .SelectRaw("COUNT(*)")
                .From(_typeInfo.TableName);

            if (!string.IsNullOrWhiteSpace(_config.DeletedColumnName))
            {
                query.WhereRaw(_config.DeletedColumnName + " = 0");
            }

            return await _db.ExecuteScalarAsync<long>(query);
        }

        private Query BuildPagedQuery(string searchTerm, IDictionary<string, string> filters, string sortColumn, string sortOrder)
        {
            var query = _db.Query()
                .Select("*")
                .From(_typeInfo.TableName);

            if (!string.IsNullOrWhiteSpace(_config.DeletedColumnName))
            {
                query.WhereRaw(_config.DeletedColumnName + " = 0");
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                ApplySearchTerm(query, searchTerm);
            }

            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    query.Where(filter.Key, filter.Value);
                }
            }

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query.OrderByRaw(sortColumn + " " + sortOrder);
            }

            return query;
        }

        private void ApplySearchTerm(Query query, string searchTerm)
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
                            query.Or().WhereRaw(columnName + " = @0", searchGuid);
                        }
                    }
                    else if (numberDataTypes.Contains(property.PropertyType) || boolDataTypes.Contains(property.PropertyType))
                    {
                        if (decimal.TryParse(searchTerm, out var searchNumber))
                        {
                            query.Or().WhereRaw(columnName + " = @0", searchNumber);
                        }
                    }
                    else if (dateDataTypes.Contains(property.PropertyType))
                    {
                        if (DateTime.TryParse(searchTerm, out var searchDate))
                        {
                            query.Or().WhereRaw(columnName + " >= @0 AND " + columnName + " < @1", searchDate.Date, searchDate.AddDays(1).Date);
                        }
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        query.Or().WhereLike(columnName, searchTerm);
                    }
                }
            }
        }
    }
}
