using Flurl.Http;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Data.SqlClient;
using UIOMatic.Front.API.ExampleCode;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using static Dapper.SqlMapper;

namespace UIOMatic.Front.API.Data
{
    public class MovieRepository : IUIOMaticRepository
    {

        public const string ApiEndPoint = "https://crudcrud.com/api/ac3d91a2c3db4cb0811613a289e82d7b/movies/";

        public object Create(object entity)
        {
            var responseString = ApiEndPoint
                   .PostJsonAsync(entity)
                   .ReceiveString().Result;
        

            return entity;
        }

        public void Delete(string[] id)
        {
            foreach (var item in id)
            {
                $"{ApiEndPoint}{id}".DeleteAsync();
                    
            }
        }

        public object Get(string id)
        {
               return $"{ApiEndPoint}{id}".GetAsync().ReceiveJson<Movie>().Result;
        }

        public IEnumerable<object> GetAll(string sortColumn = "", string sortOrder = "")
        {
            return $"{ApiEndPoint}".GetAsync().ReceiveJson<IEnumerable<Movie>>().Result;
        }

        public UIOMaticPagedResult GetPaged(int pageNumber, int itemsPerPage, string searchTerm = "", IDictionary<string, string> filters = null, string sortColumn = "", string sortOrder = "")
        {
            var all = GetAll(sortColumn, sortOrder);

            return new UIOMaticPagedResult
            {
                CurrentPage = pageNumber,
                ItemsPerPage = itemsPerPage,
                Items = all.Skip(pageNumber * itemsPerPage).Take(itemsPerPage),
                TotalItems = all.Count()

            };
        }

        public long GetTotalRecordCount()
        {
            return GetAll().Count();
        }

        public object Update(object entity)
        {
            return $"{ApiEndPoint}".PutJsonAsync(entity).ReceiveJson<Movie>().Result;
        }
    }
}
