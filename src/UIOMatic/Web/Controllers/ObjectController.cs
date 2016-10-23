using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UIOMatic.Services;
using UIOMatic.Web.PostModels;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using Umbraco.Core;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace UIOMatic.Web.Controllers
{
    [PluginController("UIOMatic")]
    public class ObjectController : UmbracoAuthorizedJsonController
    {
        private IUIOMaticObjectService _service;

        public ObjectController()
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
        public IEnumerable<object> GetFilterLookup(string typeAlias, string keyPropertyName, string valuePropertyName)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetFilterLookup(t, keyPropertyName, valuePropertyName); 
        }

        [HttpGet]
        public UIOMaticPagedResult GetPaged(string typeAlias, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, string filters, string searchTerm)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

            // Need a better approache than this as this is hacky and horrible
            // Probably want to switch to a HttpPost method and just pass a json body instead
            var filtersDict = (filters ?? "").Split('|')
                .InGroupsOf(2) 
                .ToDictionary(x => x.First(), x => x.Last())
                .Where(x => !x.Key.IsNullOrWhiteSpace() && !x.Value.IsNullOrWhiteSpace())
                .ToDictionary(x => x.Key, x => x.Value);
             
            return _service.GetPaged(t, itemsPerPage, pageNumber, sortColumn, sortOrder, filtersDict, searchTerm);
        }

        [HttpGet]
        public UIOMaticTypeInfo GetTypeInfo(string typeAlias, bool includePropertyInfo)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetTypeInfo(t, includePropertyInfo);
        }

        [HttpGet]
        public object GetById(string typeAlias, string id)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetById(t, id);
        }

        [HttpGet]
        public object GetScaffold(string typeAlias)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
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
            return _service.DeleteByIds(t, ids.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        }

        [HttpPost]
        public IEnumerable<Exception> Validate(ObjectPostModel model) 
        {
            var t = Helper.GetUIOMaticTypeByAlias(model.TypeAlias, throwNullError: true);
            return _service.Validate(t, model.Value);
        }
    }
}