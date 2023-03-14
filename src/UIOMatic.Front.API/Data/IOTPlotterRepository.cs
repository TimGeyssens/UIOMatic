using Flurl.Http;
using UIOMatic.Front.API.ExampleCode;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using static Dapper.SqlMapper;

namespace UIOMatic.Front.API.Data
{
    public class IOTPlotterRepository : IUIOMaticRepository
    {
        public const string ApiKey = "19dc4949ef417a81417dedbb86ea093a5694c743c6";
        public const string FeedId = "754620674986742536";
        public const string ApiEndPoint = "http://iotplotter.com/api/v2/feed/";
        public object Create(object entity)
        {
            if (entity is PlotterData data)
            {
                var responseString = (ApiEndPoint + FeedId)
                    .WithHeader("api-key", ApiKey)
                    .PostUrlEncodedAsync($"{data.Id},{data.GraphName},{data.Data}")
                    .ReceiveString();
            }

            return entity;
        }

        public void Delete(string[] id)
        {
            throw new NotImplementedException();
        }

        public object Get(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAll(string sortColumn = "", string sortOrder = "")
        {
            var responseString = (ApiEndPoint + FeedId)
                   .WithHeader("api-key", ApiKey)
                   .GetStringAsync().Result;

            return null;

        }

        public UIOMaticPagedResult GetPaged(int pageNumber, int itemsPerPage, string searchTerm = "", IDictionary<string, string> filters = null, string sortColumn = "", string sortOrder = "")
        {
            throw new NotImplementedException();
        }

        public long GetTotalRecordCount()
        {
            throw new NotImplementedException();
        }

        public object Update(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
