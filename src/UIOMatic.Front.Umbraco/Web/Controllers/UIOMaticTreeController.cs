using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using UIOMatic.Services;
using UIOMatic.Enums;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Trees;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.Controllers;
using Microsoft.AspNetCore.Http;
using UIOMatic.Models;
using System.Reflection;

namespace UIOMatic.Front.Umbraco.Web.Controllers
{
    [PluginController("UIOMatic")]
    public class UIOMaticTreeController : UmbracoAuthorizedApiController
    {
        private readonly IUIOMaticObjectService _objectService;
        private readonly IUIOMaticHelper _helper;
        private readonly ILogger<UIOMaticTreeController> _logger;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

        public UIOMaticTreeController(
            IUIOMaticObjectService objectService,
            IUIOMaticHelper helper,
            ILogger<UIOMaticTreeController> logger,
            ILocalizedTextService localizedTextService,
            IEventAggregator eventAggregator,
            IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
        {
            _objectService = objectService;
            _helper = helper;
            _logger = logger;
            _localizedTextService = localizedTextService;
            _eventAggregator = eventAggregator;
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        }

        [HttpGet]
        public IEnumerable<TreeNode> GetTreeNodes(string section)
        {
            var nodes = new List<TreeNode>();

            var types = _helper.GetUIOMaticTypes();
            foreach (var type in types)
            {
                var attri = type.GetCustomAttribute<UIOMaticAttribute>(true);
                if (attri == null) continue;

                if (attri.FolderName == section)
                {
                    var node = new TreeNode(
                        attri.Alias,
                        attri.FolderName,
                        attri.FolderIcon,
                        attri.ParentAlias
                    );

                    nodes.Add(node);
                }
            }

            return nodes;
        }

        [HttpGet]
        public IEnumerable<TreeNode> GetTreeNodesForType(string typeAlias)
        {
            var nodes = new List<TreeNode>();

            var type = _helper.GetUIOMaticTypes().FirstOrDefault(x => x.GetCustomAttribute<UIOMaticAttribute>(true)?.Alias == typeAlias);
            if (type == null) return nodes;

            var attri = type.GetCustomAttribute<UIOMaticAttribute>(true);
            if (attri == null) return nodes;

            var items = _objectService.GetAllAsync(type, attri.SortColumn, attri.SortOrder).GetAwaiter().GetResult();
            foreach (var item in items)
            {
                var node = new TreeNode(
                    item.GetType().GetProperty("Id")?.GetValue(item)?.ToString() ?? string.Empty,
                    attri.FolderName,
                    attri.ItemIcon,
                    attri.Alias
                );

                nodes.Add(node);
            }

            return nodes;
        }

        [HttpGet]
        public IEnumerable<MenuItem> GetMenuItems(string section)
        {
            var items = new List<MenuItem>();

            var types = _helper.GetUIOMaticTypes();
            foreach (var type in types)
            {
                var attri = type.GetCustomAttribute<UIOMaticAttribute>(true);
                if (attri == null) continue;

                if (attri.FolderName == section)
                {
                    var item = new MenuItem
                    {
                        Name = attri.ItemName,
                        Icon = attri.ItemIcon,
                        Alias = attri.Alias
                    };

                    items.Add(item);
                }
            }

            return items;
        }
    }
}
