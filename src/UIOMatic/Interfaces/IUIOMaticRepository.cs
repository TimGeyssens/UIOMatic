using System.Collections.Generic;
using UIOMatic.Models;

namespace UIOMatic.Interfaces
{
    public interface IUIOMaticRepository
    {
        IEnumerable<object> GetAll(string sortColumn = "", string sortOrder = "");

        UIOMaticPagedResult GetPaged(int pageNumber, int itemsPerPage,
            string searchTerm = "",
            IDictionary<string, string> filters = null,
            string sortColumn = "", string sortOrder = "");

        object Get(string id);

        object Create(object entity);

        object Update(object entity);

        void Delete(string[] id);
    }
}
