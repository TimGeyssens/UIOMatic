using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UIOMatic.Interfaces;
using UIOMatic.Models;

namespace UIOMatic.Data
{
    public abstract class AbstractUIOMaticRepository<TEntity, TId> : IUIOMaticRepository
    {
        public abstract IEnumerable<TEntity> GetAll(string sortColumn = "", string sortOrder = "");

        public abstract UIOMaticPagedResult<TEntity> GetPaged(
            int pageNumber,
            int itemsPerPage,
            string searchTerm = "",
            IDictionary<string, string> filters = null,
            string sortColumn = "",
            string sortOrder = "");

        public abstract TEntity Get(TId id);

        public abstract TEntity Create(TEntity entity);

        public abstract TEntity Update(TEntity entity);

        public abstract void Delete(TId[] ids);

        public abstract long GetTotalRecordCount();

        #region IUIOMaticRepository

        IEnumerable<object> IUIOMaticRepository.GetAll(string sortColumn, string sortOrder)
        {
            return GetAll(sortColumn, sortOrder).Select(x => (object)x);
        }

        UIOMaticPagedResult IUIOMaticRepository.GetPaged(
            int pageNumber,
            int itemsPerPage,
            string searchTerm,
            IDictionary<string, string> filters,
            string sortColumn,
            string sortOrder)
        {
            var r = GetPaged(pageNumber, itemsPerPage, searchTerm, filters, sortColumn, sortOrder);

            return new UIOMaticPagedResult
            {
                CurrentPage = r.CurrentPage,
                TotalPages = r.TotalPages,
                TotalItems = r.TotalItems,
                ItemsPerPage = r.ItemsPerPage,
                Items = r.Items.Select(x => (object)x)
            };
        }

        object IUIOMaticRepository.Get(string id)
        {
            return Get((TId)TypeDescriptor.GetConverter(typeof(TId)).ConvertFromInvariantString(id));
        }

        object IUIOMaticRepository.Create(object entity)
        {
            return Create((TEntity)entity);
        }

        object IUIOMaticRepository.Update(object entity)
        {
            return Update((TEntity)entity);
        }

        void IUIOMaticRepository.Delete(string[] ids)
        {
            Delete(ids.Select(x => (TId)TypeDescriptor.GetConverter(typeof(TId)).ConvertFromInvariantString(x)).ToArray());
        }

        #endregion
    }
}
