using System;
using System.Collections.Generic;
using System.Dynamic;
using UIOMatic.Models;

namespace UIOMatic.Interfaces
{
    public interface IUIOMaticObjectController
    {
        IEnumerable<object> GetAll(string typeName,string sortColumn, string sortOrder);

        IEnumerable<object> GetFiltered(string typeName, string filterColumn, string filterValue, string sortColumn, string sortOrder);

        UIOMaticPagedResult GetPaged(string typeName, int itemsPerPage, int pageNumber, string sortColumn,
            string sortOrder, string searchTerm);

        IEnumerable<UIOMaticPropertyInfo> GetAllProperties(string typeName, bool includeIgnored = false);

        IEnumerable<string> GetAllColumns(string typeName);

        UIOMaticTypeInfo GetType(string typeName);

        object GetById(string typeName, string id);

        object GetScaffold(string typeName);

        object PostCreate(ExpandoObject objectToCreate);

        object PostUpdate(ExpandoObject objectToUpdate);

        string[] DeleteByIds(string typeOfObject, string ids);

        IEnumerable<Exception> Validate(ExpandoObject objectToValidate);
    }
}
