using System;
using System.Collections.Generic;
using System.Web.Mvc;
using UIOmatic.Services;
using UIOmatic.Web.PostModels;
using UIOMatic;
using UIOMatic.Interfaces;
using UIOMatic.Models;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace UIOmatic.Web.Controllers
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
        public IEnumerable<object> GetFiltered(string typeAlias, string filterColumn, string filterValue, string sortColumn, string sortOrder)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetFiltered(t,filterColumn,filterValue, sortColumn, sortOrder);
        }

        [HttpGet]
        public UIOMaticPagedResult GetPaged(string typeAlias, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, string searchTerm)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetPaged(t, itemsPerPage, pageNumber, sortColumn, sortOrder, searchTerm);
        }

        [HttpGet]
        public IEnumerable<UIOMaticPropertyInfo> GetAllProperties(string typeAlias)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetAllProperties(t);
        }

        [HttpGet]
        public UIOMaticTypeInfo GetType(string typeAlias)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetType(t);
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