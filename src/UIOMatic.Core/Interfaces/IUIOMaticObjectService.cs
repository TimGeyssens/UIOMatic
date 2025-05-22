using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using UIOMatic.Models;

namespace UIOMatic.Interfaces
{
    public interface IUIOMaticObjectService
    {
        IEnumerable<UIOMaticPropertyInfo> GetPropertyEditors(Type type);
        IEnumerable<UIOMaticPropertyInfo> GetFields(Type type);
        IEnumerable<object> GetAllUsers();
        UIOMaticTypeInfo GetTypeInfo(Type type, bool populateProperties = false);
        object GetScaffold(Type type);
        object CreateAndPopulateType(Type type, IDictionary<string, object> values);
        object MapToObject(Type type, IDictionary<string, object> values);
        IDictionary<string, object> MapToDocument(object obj);
        IEnumerable<ValidationResult> Validate(Type type, IDictionary<string, object> values);
        void OnCreatingObject(ObjectEventArgs args);
        void OnCreatedObject(ObjectEventArgs args);
        void OnUpdatingObject(ObjectEventArgs args);
        void OnUpdatedObject(ObjectEventArgs args);
        void OnDeletingObjects(DeleteEventArgs args);
        void OnDeletedObjects(DeleteEventArgs args);
        void OnScaffoldingObject(ObjectEventArgs args);
        void OnBuildingQuery(QueryEventArgs args);
        void OnBuiltQuery(QueryEventArgs args);

        IEnumerable<string> GetAllColumns(Type type);

        Task<object> GetByIdAsync(Type type, string id);

        Task<object> CreateAsync(Type type, IDictionary<string, object> values);

        Task<object> UpdateAsync(Type type, IDictionary<string, object> values);

        Task<string[]> DeleteByIdsAsync(Type type, string[] ids);

        Task<long> GetTotalRecordCountAsync(Type type);

        Task<IEnumerable<object>> GetAllAsync(Type type, string sortColumn, string sortOrder);

        Task<UIOMaticPagedResult> GetPagedAsync(Type type, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, IDictionary<string, string> filters, string searchTerm);
        
        Task<UIOMaticPagedResult> GetPagedWithNodeIdAsync(Type type, int nodeId, string nodeIdField, int itemsPerPage, int pageNumber, string sortColumn, string sortOrder, IDictionary<string, string> filters, string searchTerm);

        Task<IEnumerable<object>> GetFilterLookupAsync(Type type, string keyPropertyName, string valuePropertyName);
    }
}
