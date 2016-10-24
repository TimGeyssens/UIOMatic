using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UIOMatic.Models;

namespace UIOMatic.Interfaces
{
    public interface IUIOMaticObjectService
    {
        UIOMaticTypeInfo GetTypeInfo(Type type, bool populateProperties = false);

        IEnumerable<string> GetAllColumns(Type type);

        object GetScaffold(Type type);

        object GetById(Type type, string id);

        object Create(Type type, IDictionary<string, object> values);

        object Update(Type type, IDictionary<string, object> values);

        string[] DeleteByIds(Type type, string[] ids);

        IEnumerable<ValidationResult> Validate(Type type, IDictionary<string, object> values);

        IEnumerable<object> GetAll(Type type,string sortColumn, string sortOrder);

        UIOMaticPagedResult GetPaged(Type type, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, IDictionary<string, string> filters, string searchTerm);

        IEnumerable<object> GetFilterLookup(Type type, string keyPropertyName, string valuePropertyName);
    }
}
