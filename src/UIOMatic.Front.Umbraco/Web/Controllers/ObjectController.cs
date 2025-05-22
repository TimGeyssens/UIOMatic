using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UIOMatic.Attributes;
using UIOMatic.Services;
using UIOMatic.Web.PostModels;
using UIOMatic.Interfaces;
using UIOMatic.Serialization;
using Newtonsoft.Json.Serialization;
using UIOMatic.Front.Umbraco.ContentApps;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Controllers;
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
        private readonly IUIOMaticObjectService _service;
        private readonly UiomaticContentAppFactoryCollection _contentAppsFactoryCollection;
        private readonly IEnumerable<IReadOnlyUserGroup> _usergroups;
        private readonly IUIOMaticHelper _helper;
        private readonly IUmbracoMapper _umbracoMapper;

        public ObjectController(
            UiomaticContentAppFactoryCollection contentAppsFactoryCollection,
            IEnumerable<IReadOnlyUserGroup> usergroups,
            IUIOMaticHelper helper,
            IUIOMaticObjectService uioMaticObjectService,
            IUmbracoMapper umbracoMapper)
        {
            _service = uioMaticObjectService;
            _contentAppsFactoryCollection = contentAppsFactoryCollection;
            _usergroups = usergroups;
            _helper = helper;
            _umbracoMapper = umbracoMapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string typeAlias, string sortColumn, string sortOrder)
        {
            var t = _helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            var result = await _service.GetAllAsync(t, sortColumn, sortOrder);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterLookup(string typeAlias, string keyPropertyName, string valuePropertyName)
        {
            var t = _helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            var result = await _service.GetFilterLookupAsync(t, keyPropertyName, valuePropertyName);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(string typeAlias, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, string filters, string searchTerm)
        {
            var t = _helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

            var filtersDict = (filters ?? "").Split('|')
                .InGroupsOf(2)
                .ToDictionary(x => x.First(), x => x.Last())
                .Where(x => !x.Key.IsNullOrWhiteSpace() && !x.Value.IsNullOrWhiteSpace())
                .ToDictionary(x => x.Key, x => x.Value);

            var result = await _service.GetPagedAsync(t, itemsPerPage, pageNumber, sortColumn, sortOrder, filtersDict, searchTerm);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedWithNodeId(string typeAlias, int nodeId, string nodeIdField, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, string filters, string searchTerm)
        {
            var t = _helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

            var filtersDict = (filters ?? "").Split('|')
                .InGroupsOf(2)
                .ToDictionary(x => x.First(), x => x.Last())
                .Where(x => !x.Key.IsNullOrWhiteSpace() && !x.Value.IsNullOrWhiteSpace())
                .ToDictionary(x => x.Key, x => x.Value);

            var result = await _service.GetPagedWithNodeIdAsync(t, nodeId, nodeIdField, itemsPerPage, pageNumber, sortColumn, sortOrder, filtersDict, searchTerm);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetTypeInfo(string typeAlias, bool includePropertyInfo)
        {
            var t = _helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);

            var info = _service.GetTypeInfo(t, includePropertyInfo);
            var derivedInfo = _umbracoMapper.Map<UIOMaticTypeInfo>(info);
            derivedInfo.Apps = _contentAppsFactoryCollection.GetContentAppsFor(t, _usergroups);

            return Ok(derivedInfo);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(string typeAlias, string id)
        {
            var t = _helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            var result = await _service.GetByIdAsync(t, id);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetScaffold(string typeAlias)
        {
            var t = _helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            var result = _service.GetScaffold(t);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSummaryDashboardTypes()
        {
            var result = await Task.FromResult(_helper.GetUIOMaticTypes()
                .Select(x => x.GetCustomAttribute<UIOMaticAttribute>(true))
                .Where(x => x.ShowOnSummaryDashboard)
                .Select(x => new
                {
                    alias = x.Alias,
                    namePlural = x.FolderName,
                    nameSingular = x.ItemName,
                    folderIcon = x.FolderIcon,
                    renderType = x.RenderType.ToString(),
                    readOnly = x.ReadOnly
                }));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ObjectPostModel model)
        {
            var t = _helper.GetUIOMaticTypeByAlias(model.TypeAlias, throwNullError: true);
            var result = await _service.CreateAsync(t, model.Value);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ObjectPostModel model)
        {
            var t = _helper.GetUIOMaticTypeByAlias(model.TypeAlias, throwNullError: true);
            var result = await _service.UpdateAsync(t, model.Value);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteByIds(string typeAlias, string ids)
        {
            var t = _helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            var result = await _service.DeleteByIdsAsync(t, ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTotalRecordCount(string typeAlias)
        {
            var t = _helper.GetUIOMaticTypeByAlias(typeAlias, throwNullError: true);
            var result = await _service.GetTotalRecordCountAsync(t);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Validate(ObjectPostModel model)
        {
            var t = _helper.GetUIOMaticTypeByAlias(model.TypeAlias, throwNullError: true);
            var result = _service.Validate(t, model.Value);
            return Ok(result);
        }
    }
}