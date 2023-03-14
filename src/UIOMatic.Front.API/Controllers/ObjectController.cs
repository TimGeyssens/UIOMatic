using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using UIOMatic.Attributes;
using UIOMatic.Services;
using UIOMatic.Web.PostModels;
using UIOMatic.Interfaces;
using UIOMatic.Serialization;
using Newtonsoft.Json.Serialization;

using Microsoft.AspNetCore.Mvc;
using UIOMatic.Front.API;
using System.Reflection;
using UIOMatic.Models;
using Microsoft.AspNetCore.Cors;

namespace UIOMatic.Front.Umbraco.Web.Controllers
{
    [ApiController]
    [Route("Object")]
    public class ObjectController : ControllerBase
    {
        private IUIOMaticObjectService _service;
        private readonly IUIOMaticHelper Helper;

        public ObjectController(
            IUIOMaticHelper helper,
            IUIOMaticObjectService uioMaticObjectService)
        {
            _service = uioMaticObjectService;
            Helper = helper;
        }

        [HttpGet]
        [Route("GetAll")]
        public IEnumerable<object> GetAll(string typeAlias, string sortColumn, string sortOrder)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetAll(t, sortColumn, sortOrder);
        }

        //[HttpGet]
        //public IEnumerable<object> GetFilterLookup(string typeAlias, string keyPropertyName, string valuePropertyName)
        //{
        //    var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
        //    return _service.GetFilterLookup(t, keyPropertyName, valuePropertyName);
        //}

        [HttpGet]
        [Route("GetPaged")]
        public UIOMatic.Models.UIOMaticPagedResult GetPaged(string typeAlias, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, string filters, string searchTerm)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);



            // Need a better approache than this as this is hacky and horrible
            // Probably want to switch to a HttpPost method and just pass a json body instead
            //var filtersDict = (filters ?? "").Split('|')
            //    .InGroupsOf(2)
            //    .ToDictionary(x => x.First(), x => x.Last())
            //    .Where(x => !x.Key.IsNullOrWhiteSpace() && !x.Value.IsNullOrWhiteSpace())
            //    .ToDictionary(x => x.Key, x => x.Value);

          

            return _service.GetPaged(t, itemsPerPage, pageNumber, sortColumn, sortOrder, null, searchTerm);
        }
        //[HttpGet]
        //public UIOMatic.Models.UIOMaticPagedResult GetPagedWithNodeId(string typeAlias, int nodeId, string nodeIdField, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, string filters, string searchTerm)
        //{
        //    var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

        //    // Need a better approache than this as this is hacky and horrible
        //    // Probably want to switch to a HttpPost method and just pass a json body instead
        //    var filtersDict = (filters ?? "").Split('|')
        //        .InGroupsOf(2)
        //        .ToDictionary(x => x.First(), x => x.Last())
        //        .Where(x => !x.Key.IsNullOrWhiteSpace() && !x.Value.IsNullOrWhiteSpace())
        //        .ToDictionary(x => x.Key, x => x.Value);

        //   // Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new UIOMaticSerializerContractResolver();

        //    return _service.GetPagedWithNodeId(t,nodeId,nodeIdField, itemsPerPage, pageNumber, sortColumn, sortOrder, filtersDict, searchTerm);
        //}


        [HttpGet]
        [Route("GetTypeInfo")]
        public UIOMaticTypeInfo GetTypeInfo(string typeAlias, bool includePropertyInfo)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

            var info = _service.GetTypeInfo(t, includePropertyInfo);


            return info;
        }

        [HttpGet]
        [Route("GetById")]
        public object GetById(string typeAlias, string id)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

            return _service.GetById(t, id);
        }

        [HttpGet]
        [Route("GetScaffold")]
        public object GetScaffold(string typeAlias)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

            // Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new UIOMaticSerializerContractResolver();

            return _service.GetScaffold(t);
        }

        [HttpGet]
        [Route("GetSummaryDashboardTypes")]
        public object GetSummaryDashboardTypes()
        {
            // Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DefaultContractResolver();

            return Helper.GetUIOMaticTypes().Select(x => x.GetCustomAttribute<UIOMaticAttribute>(false))
                .Where(x => x.ShowOnSummaryDashboard)
                .Select(x => new
                {
                    alias = x.Alias,
                    namePlural = x.FolderName,
                    nameSingular = x.ItemName,
                    folderIcon = x.FolderIcon,
                    renderType = x.RenderType.ToString(),
                    readOnly = x.ReadOnly
                });
        }

        [HttpPost]
        [Route("Create")]
        public object Create(ObjectPostModel model)
        {
            var t = Helper.GetUIOMaticTypeByAlias(model.TypeAlias, throwNullError: true);
            return _service.Create(t, model.Value);
        }

        [HttpPut]
        [Route("Update")]
        public object Update(ObjectPostModel model)
        {
            var t = Helper.GetUIOMaticTypeByAlias(model.TypeAlias, throwNullError: true);
            return _service.Update(t, model.Value);
        }

        [HttpDelete]
        [Route("Delete")]
        public string[] DeleteByIds(string typeAlias, string ids)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.DeleteByIds(t, ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        }

        [HttpGet]
        [Route("GetTotalRecordCount")]
        public object GetTotalRecordCount(string typeAlias)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetTotalRecordCount(t);
        }

        [HttpPost]
        [Route("Validate")]
        public IEnumerable<ValidationResult> Validate(ObjectPostModel model)
        {
            var t = Helper.GetUIOMaticTypeByAlias(model.TypeAlias, throwNullError: true);

            return _service.Validate(t, model.Value);
        }

    }
}