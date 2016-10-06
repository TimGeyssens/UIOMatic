using System;
using System.Collections.Generic;
using UIOMatic.Models;

namespace UIOMatic.Interfaces
{
    public interface IUIOMaticObjectService
    {
        IEnumerable<object> GetAll(Type type,string sortColumn, string sortOrder);

        IEnumerable<object> GetFiltered(Type type, string filterColumn, string filterValue, string sortColumn, string sortOrder);

        UIOMaticPagedResult GetPaged(Type type, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, string searchTerm);

        IEnumerable<UIOMaticPropertyInfo> GetAllProperties(Type type, bool includeIgnored = false);

        IEnumerable<string> GetAllColumns(Type type);

        UIOMaticTypeInfo GetType(Type type);

        object GetById(Type type, string id);

        object GetScaffold(Type type);

        object Create(Type type, IDictionary<string, object> values);

        object Update(Type type, IDictionary<string, object> values);

        string[] DeleteByIds(Type type, string ids);

        IEnumerable<Exception> Validate(Type type, IDictionary<string, object> values);
    }
}
