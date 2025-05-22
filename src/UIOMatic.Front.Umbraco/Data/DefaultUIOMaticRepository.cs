using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NPoco;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Infrastructure.Persistence;
using UIOMatic.Models;
using UIOMatic.Interfaces;
using Umbraco.Extensions;

namespace UIOMatic.Front.Umbraco.Data
{
    public class DefaultUIOMaticRepository : IUIOMaticRepository
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly ISqlContext _sqlContext;
        private readonly Type _entityType;
        private readonly string _primaryKey;

        public DefaultUIOMaticRepository(IScopeProvider scopeProvider, ISqlContext sqlContext, Type entityType, string primaryKey = "Id")
        {
            _scopeProvider = scopeProvider;
            _sqlContext = sqlContext;
            _entityType = entityType;
            _primaryKey = primaryKey;
        }

        private string GetTableName()
        {
            return _entityType.Name;
        }

        public async Task<IEnumerable<object>> GetAllAsync(string sortColumn = "", string sortOrder = "")
        {
            using var scope = _scopeProvider.CreateScope();
            var sql = new Sql().Select("*").From(_entityType.Name);
            if (!string.IsNullOrEmpty(sortColumn))
            {
                sql = sql.OrderBy($"{sortColumn} {sortOrder}");
            }
            var result = await scope.Database.FetchAsync<object>(sql);
            scope.Complete();
            return result;
        }

        public async Task<UIOMaticPagedResult> GetPagedAsync(int pageNumber, int itemsPerPage, string searchTerm = "", IDictionary<string, string> filters = null, string sortColumn = "", string sortOrder = "")
        {
            using var scope = _scopeProvider.CreateScope();
            var sql = new Sql().Select("*").From(_entityType.Name);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                sql = sql.Where("Name LIKE @0", $"%{searchTerm}%");
            }
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    sql = sql.Where($"{filter.Key} = @0", filter.Value);
                }
            }
            if (!string.IsNullOrEmpty(sortColumn))
            {
                sql = sql.OrderBy($"{sortColumn} {sortOrder}");
            }
            var page = await scope.Database.PageAsync<object>(pageNumber, itemsPerPage, sql);
            scope.Complete();
            return new UIOMaticPagedResult
            {
                CurrentPage = page.CurrentPage,
                ItemsPerPage = page.ItemsPerPage,
                TotalItems = page.TotalItems,
                TotalPages = page.TotalPages,
                Items = page.Items
            };
        }

        public async Task<object> GetAsync(string id)
        {
            using var scope = _scopeProvider.CreateScope();
            var sql = new Sql().Select("*").From(_entityType.Name).Where($"{_primaryKey} = @0", id);
            var result = await scope.Database.FirstOrDefaultAsync<object>(sql);
            scope.Complete();
            return result;
        }

        public async Task<object> CreateAsync(object entity)
        {
            using var scope = _scopeProvider.CreateScope();
            await scope.Database.InsertAsync(entity);
            scope.Complete();
            return entity;
        }

        public async Task<object> UpdateAsync(object entity)
        {
            using var scope = _scopeProvider.CreateScope();
            await scope.Database.UpdateAsync(entity);
            scope.Complete();
            return entity;
        }

        public async Task DeleteAsync(string[] ids)
        {
            using var scope = _scopeProvider.CreateScope();
            foreach (var id in ids)
            {
                var sql = new Sql().From(_entityType.Name).Where($"{_primaryKey} = @0", id);
                await scope.Database.DeleteAsync(sql);
            }
            scope.Complete();
        }

        public async Task<long> GetTotalRecordCountAsync()
        {
            using var scope = _scopeProvider.CreateScope();
            var tableName = GetTableName();
            var sql = new Sql().Select("COUNT(*)").From(tableName);
            var result = await scope.Database.ExecuteScalarAsync<long>(sql);
            scope.Complete();
            return result;
        }
    }
}
