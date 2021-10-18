using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UIOMatic.Interfaces;
using UIOMatic.Serialization;
using UIOMatic.Services;
using UIOMatic.Web.PostModels;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace UIOMatic.Web.Controllers
{
    [PluginController("UIOMatic")]
    public class PublicObjectController : UmbracoApiController
    {
        private IUIOMaticObjectService _service;

        public PublicObjectController()
        {
            _service = UIOMaticObjectService.Instance;
        }

        [HttpGet]
        public IEnumerable<object> GetAll(string typeAlias, string sortColumn, string sortOrder)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetAll(t, sortColumn, sortOrder);
        }

        [HttpGet]
        public object GetById(string typeAlias, string id)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

            Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new UIOMaticSerializerContractResolver();

            return _service.GetById(t, id);
        }

        [HttpGet]
        public object GetScaffold(string typeAlias)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

            Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new UIOMaticSerializerContractResolver();

            return _service.GetScaffold(t);
        }

        [HttpPost]
        public object Create(ObjectPostModel model)
        {
            var t = Helper.GetUIOMaticTypeByAlias(model.TypeAlias, throwNullError: true);
            return _service.Create(t, model.Value);
        }

        [HttpPost]
        public object Update(ObjectPostModel model)
        {
            var t = Helper.GetUIOMaticTypeByAlias(model.TypeAlias, throwNullError: true);
            return _service.Update(t, model.Value);
        }

        [HttpDelete]
        public string[] DeleteByIds(string typeAlias, string ids)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.DeleteByIds(t, ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        }

    }
}