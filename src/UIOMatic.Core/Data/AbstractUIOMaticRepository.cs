using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UIOMatic.Interfaces;
using UIOMatic.Models;

namespace UIOMatic.Data
{
    public abstract class AbstractUIOMaticRepository<TEntity, TId> : IUIOMaticRepository
    {
        public abstract Task<IEnumerable<TEntity>> GetAllAsync(string sortColumn = "", string sortOrder = "");

        public abstract Task<UIOMaticPagedResult<TEntity>> GetPagedAsync(
            int pageNumber,
            int itemsPerPage,
            string searchTerm = "",
            IDictionary<string, string> filters = null,
            string sortColumn = "",
            string sortOrder = "");

        public abstract Task<TEntity> GetAsync(TId id);

        public abstract Task<TEntity> CreateAsync(TEntity entity);

        public abstract Task<TEntity> UpdateAsync(TEntity entity);

        public abstract Task DeleteAsync(TId[] ids);

        public abstract Task<long> GetTotalRecordCountAsync();

        #region IUIOMaticRepository

        async Task<IEnumerable<object>> IUIOMaticRepository.GetAllAsync(string sortColumn, string sortOrder)
        {
            var result = await GetAllAsync(sortColumn, sortOrder);
            return result.Select(x => (object)x);
        }

        async Task<UIOMaticPagedResult> IUIOMaticRepository.GetPagedAsync(
            int pageNumber,
            int itemsPerPage,
            string searchTerm,
            IDictionary<string, string> filters,
            string sortColumn,
            string sortOrder)
        {
            var r = await GetPagedAsync(pageNumber, itemsPerPage, searchTerm, filters, sortColumn, sortOrder);

            return new UIOMaticPagedResult
            {
                CurrentPage = r.CurrentPage,
                TotalPages = r.TotalPages,
                TotalItems = r.TotalItems,
                ItemsPerPage = r.ItemsPerPage,
                Items = r.Items.Select(x => (object)x)
            };
        }

        async Task<object> IUIOMaticRepository.GetAsync(string id)
        {
            var convertedId = (TId)TypeDescriptor.GetConverter(typeof(TId)).ConvertFromInvariantString(id);
            return await GetAsync(convertedId);
        }

        async Task<object> IUIOMaticRepository.CreateAsync(object entity)
        {
            return await CreateAsync((TEntity)entity);
        }

        async Task<object> IUIOMaticRepository.UpdateAsync(object entity)
        {
            return await UpdateAsync((TEntity)entity);
        }

        async Task IUIOMaticRepository.DeleteAsync(string[] ids)
        {
            var convertedIds = ids.Select(x => (TId)TypeDescriptor.GetConverter(typeof(TId)).ConvertFromInvariantString(x)).ToArray();
            await DeleteAsync(convertedIds);
        }

        #endregion
    }
}
