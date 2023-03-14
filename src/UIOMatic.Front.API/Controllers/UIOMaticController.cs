using Microsoft.AspNetCore.Mvc;
using SqlKata;
using System.Reflection;
using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using UIOMatic.Models;

namespace UIOMatic.Front.API.Controllers
{
    [ApiController]
    [Route("UIOMatic")]
    public class UIOMaticController : ControllerBase
    {
        private IUIOMaticObjectService _service;
        private readonly IUIOMaticHelper Helper;

        public UIOMaticController(
            IUIOMaticHelper helper,
            IUIOMaticObjectService uioMaticObjectService)
        {
            _service = uioMaticObjectService;
            Helper = helper;
        }

        [HttpGet]
        [Route("GetAll")]
        public IEnumerable<UIOMaticTypeInfo> GetAllTypes()
        {
            var types = Helper.GetUIOMaticTypes();

            var UIOMaticTypes = new List<UIOMaticTypeInfo>();

            foreach (var type in types)
            {
                var attri = (UIOMaticFolderAttribute)Attribute.GetCustomAttribute(type, typeof(UIOMaticFolderAttribute));

                var t = Helper.GetUIOMaticTypeByAlias(attri.Alias, throwNullError: true);

                UIOMaticTypes.Add(_service.GetTypeInfo(t, true));


            }
            return UIOMaticTypes;
        }
    }
}
