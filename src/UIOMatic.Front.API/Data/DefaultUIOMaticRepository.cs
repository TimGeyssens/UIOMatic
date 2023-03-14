using System;
using System.Collections.Generic;
using System.Linq;
using UIOMatic.Extensions;
using UIOMatic.Interfaces;
using UIOMatic.Services;
using UIOMatic.Attributes;
using UIOMatic.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using SqlKata;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Newtonsoft.Json.Linq;
using static Dapper.SqlMapper;
using UIOMatic.Front.API.Extensions;

namespace UIOMatic.Front.API.Data
{
    public class DefaultUIOMaticRepository : IUIOMaticRepository
    {
        private UIOMaticAttribute _config;
        private UIOMaticTypeInfo _typeInfo;
        private readonly UIOMaticObjectService _uioMaticObjectService;
        private readonly IDbConnection _conn;
        private readonly QueryFactory _db;

        public DefaultUIOMaticRepository(UIOMaticAttribute config,
            UIOMaticTypeInfo typeInfo,
            UIOMaticObjectService uioMaticObjectService,
            QueryFactory db) : this()
        {
            _config = config;
            _typeInfo = typeInfo;
            _uioMaticObjectService = uioMaticObjectService;
            _db = db;

     

        }

        public DefaultUIOMaticRepository()
        {

        }

        public virtual IEnumerable<object> GetAll(string sortColumn = "", string sortOrder = "")
        {
            var q = _db.Query();

            var query = q.Select("*").From(_typeInfo.TableName);

            var result = _db.Compiler.Compile(query);

            var a1 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, result.Sql, sortColumn, sortOrder, "", null);
            _uioMaticObjectService.OnBuildingQuery(a1);
            query = _db.Query().SelectRaw(a1.Query);
           
            if (!string.IsNullOrWhiteSpace(this._config.DeletedColumnName))
            {
                query.WhereRaw(this._config.DeletedColumnName + " = 0");
            }

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query.OrderByRaw(sortColumn + " " + sortOrder);
            }

            result = _db.Compiler.Compile(query);

            var a2 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, result.Sql.Remove(0,6), sortColumn, sortOrder, "", null);
            _uioMaticObjectService.OnBuiltQuery(a2);
            return _db.Select(a2.Query);

              
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



            var q = _db.Query();

            var query = q.Select("*").From(_typeInfo.TableName);

            //var result = _db.Compiler.Compile(query);
            //var a1 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, result.Sql, sortColumn, sortOrder, "", null);
            //_uioMaticObjectService.OnBuildingQuery(a1);
            //query = _db.Query().SelectRaw(a1.Query);

            if (!this._config.DeletedColumnName.IsNullOrWhiteSpace())
            {
                query.WhereRaw(this._config.DeletedColumnName + " = 0");
            }
            else
            {
                query.WhereRaw("1=1");
            }

            //Filter by search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                //query.whw("AND (1=0");

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

                        //guid
                        else if (guidDataTypes.Contains(property.PropertyType))
                        {
                            Guid searchGuid;
                            if (Guid.TryParse(searchTerm, out searchGuid))
                            {
                                query.Or().WhereRaw(columnName + " = @0", searchGuid);
                            }
                        }
                        //number / boolean
                        else if (numberDataTypes.Contains(property.PropertyType) || boolDataTypes.Contains(property.PropertyType))
                        {
                            decimal searchNumber;
                            if (decimal.TryParse(searchTerm, out searchNumber))
                            {
                                query.Or().WhereRaw(columnName + " = @0", searchNumber);
                            }
                        }
                        //date
                        else if (dateDataTypes.Contains(property.PropertyType))
                        {
                            DateTime searchDate;
                            if (DateTime.TryParse(searchTerm, out searchDate))
                            {
                                query.Or().WhereRaw(columnName + " >=  @0 AND " + columnName + " < @1", searchDate.Date, searchDate.AddDays(1).Date);
                            }
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            query.Or().WhereLike(columnName, searchTerm);
                        }

                        c++;
                    }
                }

                //query.Append(")");
            }

            //if (filters != null && filters.Any())
            //{
            //    foreach (var filter in filters)
            //    {
            //        query.Append("AND " + filter.Key + " = @0", filter.Value);
            //    }
            //}

            //Sort


            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
            {
                query.OrderByRaw("[" + sortColumn + "] " + sortOrder);
            }
            else if (!string.IsNullOrEmpty(_config.SortColumn) && !string.IsNullOrEmpty(_config.SortOrder))
            {
                query.OrderByRaw("[" + _config.SortColumn + "] " + _config.SortOrder);
            }
            else
            {
                var primaryKeyColum = "Id";/* _typeInfo.Type.GetPrimaryKeyName()*/;
                query.OrderByRaw("[" + primaryKeyColum + "] asc");
            }

            //result = _db.Compiler.Compile(query);

            //var a2 = new QueryEventArgs(_typeInfo.Type, _typeInfo.TableName, result.Sql.Remove(0, 6), sortColumn, sortOrder, "", null);
            //_uioMaticObjectService.OnBuiltQuery(a2);
            //query = _db.Query().SelectRaw(a2.Query);

            var p = query.Paginate(pageNumber, itemsPerPage);
          

            return new UIOMaticPagedResult
            {
                CurrentPage = p.Page,
                ItemsPerPage = p.PerPage,
                TotalItems = p.Count,
                TotalPages = p.TotalPages,
                Items = p.List
            };
            
        }

        public virtual object Get(string id)
        {
            var q = _db.Query();

            var query = q.Select("*").From(_typeInfo.TableName);
            query.WhereRaw("id" + " = " + id);
            return _db.Get(query).FirstOrDefault();


           

            //    var query = new Sql().Select("*").From(_typeInfo.TableName);
            //    var table = NPoco.TableInfo.FromPoco(_typeInfo.Type);
            //    query.Append("WHERE " + table.PrimaryKey + " = " + id);

            //    return scope.Database.Query(_typeInfo.Type, query.SQL).FirstOrDefault();

        }

        public virtual object Create(object entity)
        {

            var values = entity.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.Name != "Id")
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(entity, null));

            var id = _db.Query(_typeInfo.TableName).InsertGetId<int>(values);

            //if (!this._typeInfo.DateCreatedFieldKey.IsNullOrWhiteSpace())
            //{
            //    entity.SetPropertyValue(this._typeInfo.DateCreatedFieldKey, DateTime.Now);
            //}

            //if (!this._typeInfo.DateModifiedFieldKey.IsNullOrWhiteSpace())
            //{
            //    entity.SetPropertyValue(this._typeInfo.DateModifiedFieldKey, DateTime.Now);
            //}

            //if (_typeInfo.AutoIncrementPrimaryKey)
            //    scope.Database.Insert(_typeInfo.TableName, _typeInfo.PrimaryKeyColumnName, true, entity);
            //else
            //    scope.Database.Insert(entity);
            values.Add("Id", id);
            return values;
            
        }

        public virtual object Update(object entity)
        {

            var values = entity.GetType()
               .GetProperties(BindingFlags.Instance | BindingFlags.Public)
               .Where(x => x.Name != "Id")
               .ToDictionary(prop => prop.Name, prop => prop.GetValue(entity, null));

            var id = entity.GetType()
               .GetProperties(BindingFlags.Instance | BindingFlags.Public)
               .FirstOrDefault(x => x.Name == "Id");

            _db.Query(_typeInfo.TableName).Where(id.Name,id.GetValue(entity,null)).Update(values);
            //        if (!this._typeInfo.DateModifiedFieldKey.IsNullOrWhiteSpace())
            //        {
            //            entity.SetPropertyValue(this._typeInfo.DateModifiedFieldKey, DateTime.Now);
            //        }
            return entity;

        }

        public virtual void Delete(string[] ids)
        {
            _db.Query(_typeInfo.TableName).WhereIn(_config.DeletedColumnName.IsNullOrWhiteSpace() ? "Id": _config.DeletedColumnName, ids).Delete();

          
        }

        public virtual long GetTotalRecordCount()
        {
            return _db.Query(_typeInfo.TableName).Count<long>();
           
          

            //    var sql = string.Format("SELECT COUNT(1) FROM {0}", _typeInfo.TableName);

            //    if (!this._config.DeletedColumnName.IsNullOrWhiteSpace())
            //    {
            //        sql += string.Format(" WHERE {0} = 0", this._config.DeletedColumnName);
            //    }

            //    return scope.Database.ExecuteScalar<long>(sql);
            //}
        }

    }
}
