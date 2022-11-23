using System;
using System.Collections.Generic;
using System.Linq;
using UIOMatic.Extensions;
using UIOMatic.Interfaces;
using UIOMatic.Services;
using UIOMatic.Attributes;
using UIOMatic.Models;
using NPoco;
using System.Configuration;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Scoping;

namespace UIOMatic.Data
{
    public class DefaultUIOMaticRepository : IUIOMaticRepository
    {
        private UIOMaticAttribute _config;
        private UIOMaticTypeInfo _typeInfo;
        private readonly UIOMaticObjectService _uioMaticObjectService;
        private readonly IScopeProvider _scopeProvider;

        public DefaultUIOMaticRepository(UIOMaticAttribute config,
            UIOMaticTypeInfo typeInfo,
            IScopeProvider scopeProvider,
            UIOMaticObjectService uioMaticObjectService) : this(scopeProvider)
        {
            _config = config;
            _typeInfo = typeInfo;
            _uioMaticObjectService = uioMaticObjectService;
        }

        public DefaultUIOMaticRepository(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public virtual IEnumerable<object> GetAll(string sortColumn = "", string sortOrder = "")
        {

            using (IScope scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var query = new Sql().Select("*").From(_typeInfo.TableName);

                var a1 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, query, sortColumn, sortOrder, "", null);
                _uioMaticObjectService.OnBuildingQuery(a1);
                query = a1.Query;

                if (!this._config.DeletedColumnName.IsNullOrWhiteSpace())
                {
                    query.Append("WHERE " + this._config.DeletedColumnName + " = 0");
                }

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
                {
                    query.OrderBy(sortColumn + " " + sortOrder);
                }

                var a2 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, query, sortColumn, sortOrder, "", null);
                _uioMaticObjectService.OnBuiltQuery(a2);
                query = a2.Query;

                return scope.Database.Fetch(_typeInfo.Type, query);
            }
        }

        public virtual UIOMaticPagedResult GetPaged(
            int pageNumber,
            int itemsPerPage,
            string searchTerm = "",
            IDictionary<string, string> filters = null,
            string sortColumn = "",
            string sortOrder = "")
        {

            var numberDataTypes = new[] {
                typeof(Byte),
                typeof(Decimal),
                typeof(Double),
                typeof(Int16),
                typeof(Int32),
                typeof(Int64),
                typeof(SByte),
                typeof(Single),
                typeof(UInt16),
                typeof(UInt32),

                typeof(Byte?),
                typeof(Decimal?),
                typeof(Double?),
                typeof(Int16?),
                typeof(Int32?),
                typeof(Int64?),
                typeof(SByte?),
                typeof(Single?),
                typeof(UInt16?),
                typeof(UInt32?)
            };

            var guidDataTypes = new[] {
                typeof(Guid),
                typeof(Guid?)
            };

            var dateDataTypes = new[] {
                typeof(DateTime),
                typeof(DateTime?)
            };

            var boolDataTypes = new[] {
                typeof(bool),
                typeof(bool?)
            };

            using (IScope scope = _scopeProvider.CreateScope(autoComplete: true))
            {

                var query = new Sql().Select("*").From(_typeInfo.TableName);

                var a1 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, query, sortColumn, sortOrder, searchTerm, filters);
                _uioMaticObjectService.OnBuildingQuery(a1);
                query = a1.Query;

                if (!this._config.DeletedColumnName.IsNullOrWhiteSpace())
                {
                    query.Append("WHERE " + this._config.DeletedColumnName + " = 0");
                }
                else
                {
                    query.Append("WHERE 1=1");
                }

                // Filter by search term
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query.Append("AND (1=0");

                    var c = 0;
                    foreach (var property in _typeInfo.Type.GetProperties())
                    {
                        var attris = property.GetCustomAttributes(true);
                        if (attris.All(x => x.GetType() != typeof(IgnoreAttribute)))
                        {
                            var columnName = property.Name;

                            var columnAttri = attris.FirstOrDefault(x => x.GetType() == typeof(ColumnAttribute)) as ColumnAttribute;
                            if (columnAttri != null)
                                columnName = columnAttri.Name;

                            // guid
                            else if (guidDataTypes.Contains(property.PropertyType))
                            {
                                Guid searchGuid;
                                if (Guid.TryParse(searchTerm, out searchGuid))
                                {
                                    query.Append("OR " + columnName + " = @0", searchGuid);
                                }
                            }
                            // number / boolean
                            else if (numberDataTypes.Contains(property.PropertyType) || boolDataTypes.Contains(property.PropertyType))
                            {
                                decimal searchNumber;
                                if (decimal.TryParse(searchTerm, out searchNumber))
                                {
                                    query.Append("OR " + columnName + " = @0", searchNumber);
                                }
                            }
                            // date
                            else if (dateDataTypes.Contains(property.PropertyType))
                            {
                                DateTime searchDate;
                                if (DateTime.TryParse(searchTerm, out searchDate))
                                {
                                    query.Append("OR " + columnName + " >=  @0 AND " + columnName + " < @1", searchDate.Date, searchDate.AddDays(1).Date);
                                }
                            }
                            else if (property.PropertyType == typeof(string))
                            {
                                query.Append("OR " + columnName + " like @0", "%" + searchTerm + "%");
                            }

                            c++;
                        }
                    }

                    query.Append(")");
                }

                if (filters != null && filters.Any())
                {
                    foreach (var filter in filters)
                    {
                        query.Append("AND " + filter.Key + " = @0", filter.Value);
                    }
                }

                // Sort
              
                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
                {
                    query.OrderBy("[" + sortColumn + "] " + sortOrder);
                }
                else if (!string.IsNullOrEmpty(_config.SortColumn) && !string.IsNullOrEmpty(_config.SortOrder))
                {
                    query.OrderBy("[" +_config.SortColumn + "] " + _config.SortOrder);
                }
                else
                {
                    var primaryKeyColum = _typeInfo.Type.GetPrimaryKeyName();
                    query.OrderBy("[" + primaryKeyColum + "] asc");
                }

                var a2 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, query, sortColumn, sortOrder, searchTerm, filters);
                _uioMaticObjectService.OnBuiltQuery(a2);
                query = a2.Query;


                var p = scope.Database.Page<object>(pageNumber, itemsPerPage, query.SQL);

                return new UIOMaticPagedResult
                {
                    CurrentPage = p.CurrentPage,
                    ItemsPerPage = p.ItemsPerPage,
                    TotalItems = p.TotalItems,
                    TotalPages = p.TotalPages,
                    Items = p.Items
                };
            }
        }

        public virtual object Get(string id)
        {
            using (IScope scope = _scopeProvider.CreateScope(autoComplete: true))
            {

                var query = new Sql().Select("*").From(_typeInfo.TableName);
                var table = NPoco.TableInfo.FromPoco(_typeInfo.Type);
                query.Append("WHERE " + table.PrimaryKey + " = " + id);

                return scope.Database.Query(_typeInfo.Type, query.SQL).FirstOrDefault();
            }
        }

        public virtual object Create(object entity)
        {
            using (IScope scope = _scopeProvider.CreateScope(autoComplete: true))
            {

                if (!this._typeInfo.DateCreatedFieldKey.IsNullOrWhiteSpace())
                {
                    entity.SetPropertyValue(this._typeInfo.DateCreatedFieldKey, DateTime.Now);
                }

                if (!this._typeInfo.DateModifiedFieldKey.IsNullOrWhiteSpace())
                {
                    entity.SetPropertyValue(this._typeInfo.DateModifiedFieldKey, DateTime.Now);
                }

                if (_typeInfo.AutoIncrementPrimaryKey)
                    scope.Database.Insert(_typeInfo.TableName, _typeInfo.PrimaryKeyColumnName, true, entity);
                else
                    scope.Database.Insert(entity);

                return entity;
            }
        }

        public virtual object Update(object entity)
        {
            using (IScope scope = _scopeProvider.CreateScope(autoComplete: true))
            {

                if (!this._typeInfo.DateModifiedFieldKey.IsNullOrWhiteSpace())
                {
                    entity.SetPropertyValue(this._typeInfo.DateModifiedFieldKey, DateTime.Now);
                }

                scope.Database.Update(entity);

                return entity;
            }
        }

        public virtual void Delete(string[] ids)
        {
            using (IScope scope = _scopeProvider.CreateScope(autoComplete: true))
            {

                if (this._config.DeletedColumnName.IsNullOrWhiteSpace())
                {
                    var sql = string.Format(
                        "DELETE FROM {0} WHERE {1} IN ({2})",
                        _typeInfo.TableName,
                        _typeInfo.PrimaryKeyColumnName,
                        string.Join(",", ids.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => "'" + x + "'")));

                    scope.Database.Execute(sql);
                }
                else
                {
                    var sql = string.Format(
                        "UPDATE {0} SET {1} = 1 WHERE {2} IN ({3})",
                        _typeInfo.TableName,
                        this._config.DeletedColumnName,
                        _typeInfo.PrimaryKeyColumnName,
                        string.Join(",", ids.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => "'" + x + "'")));

                    scope.Database.Execute(sql);
                }
            }
        }

        public virtual long GetTotalRecordCount()
        {
            using (IScope scope = _scopeProvider.CreateScope(autoComplete: true))
            {

                var sql = string.Format("SELECT COUNT(1) FROM {0}", _typeInfo.TableName);

                if (!this._config.DeletedColumnName.IsNullOrWhiteSpace())
                {
                    sql += string.Format(" WHERE {0} = 0", this._config.DeletedColumnName);
                }

                return scope.Database.ExecuteScalar<long>(sql);
            }
        }

    }
}
