using System;
using System.Collections.Generic;
using System.Linq;
using UIOMatic.Extensions;
using UIOMatic.Interfaces;
using UIOMatic.Services;
using UIOMatic.Attributes;
using UIOMatic.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace UIOMatic.Data
{
    internal class DefaultUIOMaticRepository : IUIOMaticRepository
    {
        private UIOMaticAttribute _config;
        private UIOMaticTypeInfo _typeInfo;

        public DefaultUIOMaticRepository(UIOMaticAttribute config, UIOMaticTypeInfo typeInfo)
        {
            _config = config;
            _typeInfo = typeInfo;
        }

        public IEnumerable<object> GetAll(string sortColumn = "", string sortOrder = "")
        {
            var db = GetDb();
            
            var query = new Sql().Select("*").From(_typeInfo.TableName);

            var a1 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, query, sortColumn, sortOrder, "",null);
            UIOMaticObjectService.OnBuildingQuery(a1);
            query = a1.Query;

            if (!this._config.DeletedColumnName.IsNullOrWhiteSpace())
            {
                query.Append("WHERE " + this._config.DeletedColumnName + " = 0");
            }

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query.OrderBy(sortColumn + " " + sortOrder);
            }

            var a2 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, query, sortColumn, sortOrder, "",null);
            UIOMaticObjectService.OnBuiltQuery(a2);
            query = a2.Query;

            return db.Fetch(_typeInfo.Type, query);
        }

        public UIOMaticPagedResult GetPaged(
            int pageNumber,
            int itemsPerPage,
            string searchTerm = "",
            IDictionary<string, string> filters = null,
            string sortColumn = "",
            string sortOrder = "")
        {
            var db = GetDb();

            var query = new Sql().Select("*").From(_typeInfo.TableName);

            var a1 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, query, sortColumn, sortOrder, searchTerm, filters);
            UIOMaticObjectService.OnBuildingQuery(a1);
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

                        query.Append("OR " + columnName + " like @0", "%" + searchTerm + "%");
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
            // BUG: There is a bug in the peta poco version used that errors if sort column is wrapped in [] so need to make sure it's not
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query.OrderBy(sortColumn + " " + sortOrder);
            }
            else if (!string.IsNullOrEmpty(_config.SortColumn) && !string.IsNullOrEmpty(_config.SortOrder))
            {
                query.OrderBy(_config.SortColumn + " " + _config.SortOrder);
            }
            else
            {
                var primaryKeyColum = _typeInfo.Type.GetPrimaryKeyName();
                query.OrderBy(primaryKeyColum + " asc");
            }

            var a2 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, query, sortColumn, sortOrder, searchTerm, filters);
            UIOMaticObjectService.OnBuiltQuery(a2);
            query = a2.Query;

            var p = db.Page(_typeInfo.Type, pageNumber, itemsPerPage, query);

            return new UIOMaticPagedResult
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items
            };
        }

        public object Get(string id)
        {
            var db = GetDb();

            return db.SingleOrDefault(_typeInfo.Type, id);
        }

        public object Create(object entity)
        {
            var db = GetDb();

            if (!this._typeInfo.DateCreatedFieldKey.IsNullOrWhiteSpace())
            {
                entity.SetPropertyValue(this._typeInfo.DateCreatedFieldKey, DateTime.Now);
            }

            if (!this._typeInfo.DateModifiedFieldKey.IsNullOrWhiteSpace())
            {
                entity.SetPropertyValue(this._typeInfo.DateModifiedFieldKey, DateTime.Now);
            }

            if (_typeInfo.AutoIncrementPrimaryKey)
                db.Insert(_typeInfo.TableName, _typeInfo.PrimaryKeyColumnName, true, entity);
            else
                db.Insert(entity);

            return entity;
        }

        public object Update(object entity)
        {
            var db = GetDb();
             
            if (!this._typeInfo.DateModifiedFieldKey.IsNullOrWhiteSpace())
            {
                entity.SetPropertyValue(this._typeInfo.DateModifiedFieldKey, DateTime.Now);
            }

            db.Update(entity);

            return entity;
        }

        public void Delete(string[] ids)
        {
            var db = GetDb();

            if (this._config.DeletedColumnName.IsNullOrWhiteSpace())
            {
                var sql = string.Format(
                    "DELETE FROM {0} WHERE {1} IN ({2})",
                    _typeInfo.TableName,
                    _typeInfo.PrimaryKeyColumnName,
                    string.Join(",", ids.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => "'" + x + "'")));

                db.Execute(sql);
            }
            else
            {
                var sql = string.Format(
                    "UPDATE {0} SET {1} = 1 WHERE {2} IN ({3})",
                    _typeInfo.TableName,
                    this._config.DeletedColumnName,
                    _typeInfo.PrimaryKeyColumnName,
                    string.Join(",", ids.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => "'" + x + "'")));

                db.Execute(sql);
            }
        }

        public long GetTotalRecordCount()
        {
            var db = GetDb();

            var sql = string.Format("SELECT COUNT(1) FROM {0}", _typeInfo.TableName);

            if (!this._config.DeletedColumnName.IsNullOrWhiteSpace())
            {
                sql += string.Format(" WHERE {0} = 0", this._config.DeletedColumnName);
            }

            return db.ExecuteScalar<long>(sql);
        }

        private Database GetDb()
        {
            return !string.IsNullOrEmpty(_config.ConnectionStringName)
                ? new Database(_config.ConnectionStringName)
                : ApplicationContext.Current.DatabaseContext.Database;
        }
    }
}
