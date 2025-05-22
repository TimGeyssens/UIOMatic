using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;
using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using UIOMatic.Models;

namespace UIOMatic.Front.Umbraco.Web.Controllers
{
    [PluginController("UIOMatic")]
    public class UIOMaticServerVariablesController : UmbracoAuthorizedApiController
    {
        private readonly IUIOMaticHelper _helper;
        private readonly IContentTypeService _contentTypeService;
        private readonly IUmbracoMapper _mapper;

        public UIOMaticServerVariablesController(
            IUIOMaticHelper helper,
            IContentTypeService contentTypeService,
            IUmbracoMapper mapper)
        {
            _helper = helper;
            _contentTypeService = contentTypeService;
            _mapper = mapper;
        }

        [HttpGet]
        public object GetServerVariables()
        {
            var types = _helper.GetUIOMaticTypes();
            var sections = new List<object>();

            foreach (var type in types)
            {
                var attri = type.GetCustomAttribute<UIOMaticAttribute>(true);
                if (attri == null) continue;

                var section = new
                {
                    alias = attri.Alias,
                    name = attri.ItemName,
                    icon = attri.ItemIcon,
                    folderName = attri.FolderName,
                    type = type.Name,
                    properties = GetProperties(type)
                };

                sections.Add(section);
            }

            return new
            {
                sections = sections,
                contentTypes = GetContentTypes()
            };
        }

        private IEnumerable<object> GetProperties(Type type)
        {
            var properties = type.GetProperties();
            var result = new List<object>();

            foreach (var prop in properties)
            {
                var fieldAttri = prop.GetCustomAttributes(typeof(UIOMaticFieldAttribute), true).FirstOrDefault() as UIOMaticFieldAttribute;
                if (fieldAttri == null) continue;

                var property = new
                {
                    name = prop.Name,
                    label = fieldAttri.Name,
                    description = fieldAttri.Description,
                    tab = fieldAttri.Tab,
                    tabOrder = fieldAttri.TabOrder,
                    view = fieldAttri.View,
                    config = fieldAttri.Config,
                    isNameField = fieldAttri.IsNameField,
                    order = fieldAttri.Order
                };

                result.Add(property);
            }

            return result;
        }

        private IEnumerable<object> GetContentTypes()
        {
            var contentTypes = _contentTypeService.GetAll();
            return contentTypes.Select(x => new
            {
                id = x.Id,
                alias = x.Alias,
                name = x.Name,
                icon = x.Icon,
                description = x.Description
            });
        }
    }
} 