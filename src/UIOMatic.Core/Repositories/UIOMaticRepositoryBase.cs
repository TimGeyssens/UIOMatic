using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public abstract IEnumerable<object> GetAll(string sortColumn, string sortOrder);
        public abstract object GetById(string id);
        public abstract object Get(string id);
        public abstract object Create(object entity);
        public abstract object Update(object entity);
        public abstract void Delete(string[] ids);
        public abstract IEnumerable<ValidationResult> Validate(object entity);
        public abstract long GetTotalRecordCount();
        public abstract UIOMaticPagedResult GetPaged(int pageNumber, int itemsPerPage, string searchTerm, IDictionary<string, string> filters, string sortColumn, string sortOrder);
    }
} 