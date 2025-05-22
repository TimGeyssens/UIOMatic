using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using UIOMatic.Interfaces;
using UIOMatic.Models;

namespace UIOMatic.Core.Repositories
{
    public abstract class UIOMaticRepositoryBase : IUIOMaticRepository
    {
        public virtual void Initialize(object[] parameters)
        {
            // Default implementation does nothing
            // This makes the method optional for existing repositories
        }

        public abstract Task<IEnumerable<object>> GetAllAsync(string sortColumn = "", string sortOrder = "");
        public abstract Task<object> GetAsync(string id);
        public abstract Task<object> CreateAsync(object entity);
        public abstract Task<object> UpdateAsync(object entity);
        public abstract Task DeleteAsync(string[] ids);
        public abstract Task<long> GetTotalRecordCountAsync();
        public abstract Task<UIOMaticPagedResult> GetPagedAsync(
            int pageNumber, 
            int itemsPerPage, 
            string searchTerm = "", 
            IDictionary<string, string> filters = null, 
            string sortColumn = "", 
            string sortOrder = "");
    }
} 