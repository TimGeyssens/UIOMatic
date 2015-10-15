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
    public interface IUIOMaticObjectController
    {
        IEnumerable<Object> GetAll(string typeName,string sortColumn, string sortOrder);

        IEnumerable<UIOMaticPropertyInfo> GetAllProperties(string typeName);

        Object GetById(string typeName, int id);

        Object PostCreate(ExpandoObject objectToCreate);

        Object PostUpdate(ExpandoObject objectToUpdate);

        int[] DeleteByIds(string typeOfObject, string ids);

        IEnumerable<Exception> Validate(ExpandoObject objectToValidate);
    }
}
