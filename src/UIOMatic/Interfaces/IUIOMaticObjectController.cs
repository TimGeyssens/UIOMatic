using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        UIOMaticPagedResult GetPaged(string typeName, int itemsPerPage, int pageNumber, string sortColumn,
            string sortOrder, string searchTerm);

        IEnumerable<UIOMaticPropertyInfo> GetAllProperties(string typeName, bool includeIgnored = false);

        IEnumerable<string> GetAllColumns(string typeName);

        UIOMaticTypeInfo GetType(string typeName);

        Object GetById(string typeName, int id);

        Object PostCreate(ExpandoObject objectToCreate);

        Object PostUpdate(ExpandoObject objectToUpdate);

        int[] DeleteByIds(string typeOfObject, string ids);

        IEnumerable<Exception> Validate(ExpandoObject objectToValidate);
    }
}
