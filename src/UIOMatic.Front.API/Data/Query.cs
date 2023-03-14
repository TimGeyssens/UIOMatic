using Humanizer;
using Newtonsoft.Json;
using SqlKata;
using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using UIOMatic.Interfaces;
using UIOMatic.Site.ExampleCode;

namespace UIOMatic.Front.API.Data
{
    public class Query
    {      
        public IEnumerable<Person> GetAllPersons([Service] IUIOMaticHelper helper, [Service] IUIOMaticObjectService uioMaticObjectService,
            string sortColumn, string sortOrder)
        {
            var t = helper.GetUIOMaticTypeByAlias("people", throwNullError: true);
            return uioMaticObjectService.GetAll(t, sortColumn, sortOrder).Select(x =>
             JsonConvert.DeserializeObject<Person>(JsonConvert.SerializeObject(x)));

        }

        public Person GetPerson([Service] IUIOMaticHelper helper, [Service] IUIOMaticObjectService uioMaticObjectService,
           string id)
        {
            var t = helper.GetUIOMaticTypeByAlias("people", throwNullError: true);
            return JsonConvert.DeserializeObject<Person>(uioMaticObjectService.GetById(t,id).ToString());
        }

    }
}
