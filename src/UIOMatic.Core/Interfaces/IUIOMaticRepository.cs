using System.Collections.Generic;
using System.Threading.Tasks;
using UIOMatic.Models;

namespace UIOMatic.Interfaces
{
    public interface IUIOMaticRepository
    {
        Task<IEnumerable<object>> GetAllAsync(string sortColumn = "", string sortOrder = "");

        Task<UIOMaticPagedResult> GetPagedAsync(
            int pageNumber, 
            int itemsPerPage,
            string searchTerm = "",
            IDictionary<string, string> filters = null,
            string sortColumn = "", 
            string sortOrder = "");
    
        Task<object> GetAsync(string id);

        Task<object> CreateAsync(object entity);

        Task<object> UpdateAsync(object entity);

        Task DeleteAsync(string[] ids);

        Task<long> GetTotalRecordCountAsync();
    }
}
