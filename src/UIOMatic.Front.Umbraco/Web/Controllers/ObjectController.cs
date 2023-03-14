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
using UIOMatic.Front.Umbraco.ContentApps;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models.Membership;
using UIOMatic.Front.Umbraco.Models;
using System.IO;
using Umbraco.Cms.Core.Mapping;

namespace UIOMatic.Front.Umbraco.Web.Controllers
{
    [PluginController("UIOMatic")]
    public class ObjectController : UmbracoAuthorizedJsonController
    {
        private IUIOMaticObjectService _service;
        private readonly UiomaticContentAppFactoryCollection _contentAppsFactoryCollection;
        private readonly IEnumerable<IReadOnlyUserGroup> _usergroups;
        private readonly IUIOMaticHelper Helper;
        private readonly IUmbracoMapper _umbracoMapper;

        public ObjectController(UiomaticContentAppFactoryCollection contentAppsFactoryCollection, 
            IEnumerable<IReadOnlyUserGroup> usergroups,
            IUIOMaticHelper helper,
            IUIOMaticObjectService uioMaticObjectService,
            IUmbracoMapper umbracoMapper)
        {
            _service = uioMaticObjectService;
            _contentAppsFactoryCollection = contentAppsFactoryCollection;
            _usergroups = usergroups;
            Helper = helper;
            _umbracoMapper = umbracoMapper;
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
        public UIOMatic.Models.UIOMaticPagedResult GetPaged(string typeAlias, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, string filters, string searchTerm)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

           

            // Need a better approache than this as this is hacky and horrible
            // Probably want to switch to a HttpPost method and just pass a json body instead
            var filtersDict = (filters ?? "").Split('|')
                .InGroupsOf(2) 
                .ToDictionary(x => x.First(), x => x.Last())
                .Where(x => !x.Key.IsNullOrWhiteSpace() && !x.Value.IsNullOrWhiteSpace())
                .ToDictionary(x => x.Key, x => x.Value);

            //Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new UIOMaticSerializerContractResolver();

            return _service.GetPaged(t, itemsPerPage, pageNumber, sortColumn, sortOrder, filtersDict, searchTerm);
        }
        [HttpGet]
        public UIOMatic.Models.UIOMaticPagedResult GetPagedWithNodeId(string typeAlias, int nodeId, string nodeIdField, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, string filters, string searchTerm)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

            // Need a better approache than this as this is hacky and horrible
            // Probably want to switch to a HttpPost method and just pass a json body instead
            var filtersDict = (filters ?? "").Split('|')
                .InGroupsOf(2)
                .ToDictionary(x => x.First(), x => x.Last())
                .Where(x => !x.Key.IsNullOrWhiteSpace() && !x.Value.IsNullOrWhiteSpace())
                .ToDictionary(x => x.Key, x => x.Value);

           // Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new UIOMaticSerializerContractResolver();

            return _service.GetPagedWithNodeId(t,nodeId,nodeIdField, itemsPerPage, pageNumber, sortColumn, sortOrder, filtersDict, searchTerm);
        }


        [HttpGet]
        public UIOMaticTypeInfo GetTypeInfo(string typeAlias, bool includePropertyInfo)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

            //Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DefaultContractResolver();

            var info = _service.GetTypeInfo(t, includePropertyInfo);
            var derivedInfo = _umbracoMapper.Map<UIOMaticTypeInfo>(info);
            derivedInfo.Apps = _contentAppsFactoryCollection.GetContentAppsFor(t, _usergroups);

            return derivedInfo;
        }

        [HttpGet]
        public object GetById(string typeAlias, string id)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

            //Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new UIOMaticSerializerContractResolver();

            return _service.GetById(t, id);
        }

        [HttpGet]
        public object GetScaffold(string typeAlias)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

           // Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new UIOMaticSerializerContractResolver();

            return _service.GetScaffold(t);
        }

        [HttpGet]
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

        [HttpGet]
        public object GetTotalRecordCount(string typeAlias)
        {
            var t = Helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            return _service.GetTotalRecordCount(t);
        }

        [HttpPost]
        public IEnumerable<ValidationResult> Validate(ObjectPostModel model) 
        {
            var t = Helper.GetUIOMaticTypeByAlias(model.TypeAlias, throwNullError: true);

           // Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DefaultContractResolver();

            return _service.Validate(t, model.Value);
        }

    }
}