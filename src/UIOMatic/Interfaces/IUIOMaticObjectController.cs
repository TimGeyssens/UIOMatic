using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UIOMatic.Models;

namespace UIOMatic.Interfaces
{
    interface IUIOMaticObjectController
    {
        IEnumerable<Object> GetAll(string typeName);

        IEnumerable<UIOMaticPropertyInfo> GetAllProperties(string typeName);

        Object GetById(string typeName, int id);

        Object PostCreate(ExpandoObject objectToCreate);

        Object PostUpdate(ExpandoObject objectToUpdate);

        int DeleteById(string typeOfObject, int id);

        IEnumerable<Exception> Validate(ExpandoObject objectToValidate);
    }
}
