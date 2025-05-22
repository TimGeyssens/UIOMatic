using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Expressions;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseModelDefinitions;
using Umbraco.Extensions;
using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using NPoco;

namespace UIOMatic.Front.Umbraco.Migrations
{
    /// <summary>
    /// Base class for UIOMatic migrations that provides common functionality
    /// </summary>
    public abstract class UIOMaticMigrationBase : MigrationBase
    {
        protected readonly IUIOMaticHelper _helper;

        protected UIOMaticMigrationBase(IMigrationContext context, IUIOMaticHelper helper) 
            : base(context)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        /// <summary>
        /// Ensures a table exists for a UIOMatic type
        /// </summary>
        /// <typeparam name="T">The UIOMatic type</typeparam>
        protected void EnsureTableExists<T>() where T : class
        {
            var type = typeof(T);
            var attr = type.GetCustomAttribute<UIOMaticAttribute>();
            
            if (attr == null)
            {
                throw new InvalidOperationException($"Type {type.Name} is not marked with UIOMaticAttribute");
            }

            if (!TableExists(attr.Alias))
            {
                Create.Table(attr.Alias)
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .Do();
            }
        }

        /// <summary>
        /// Ensures a column exists in a table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="columnName">The name of the column</param>
        /// <param name="columnType">The SQL type of the column</param>
        protected void EnsureColumnExists(string tableName, string columnName, System.Data.DbType columnType)
        {
            if (!ColumnExists(tableName, columnName))
            {
                Create.Column(columnName)
                    .OnTable(tableName)
                    .AsCustom(columnType.ToString())
                    .Do();
            }
        }

        /// <summary>
        /// Ensures an index exists on a table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="indexName">The name of the index</param>
        /// <param name="columnName">The name of the column to index</param>
        protected void EnsureIndexExists(string tableName, string indexName, string columnName)
        {
            if (!IndexExists(indexName))
            {
                Create.Index(indexName)
                    .OnTable(tableName)
                    .OnColumn(columnName)
                    .Ascending()
                    .Do();
            }
        }

        protected void CreateTable(Type type)
        {
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null) return;

            var tableName = type.Name;
            if (!TableExists(tableName))
            {
                var createTable = Create.Table(tableName)
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity();
                var properties = type.GetProperties();
                foreach (var prop in properties)
                {
                    var fieldAttri = prop.GetCustomAttribute<UIOMaticFieldAttribute>();
                    if (fieldAttri == null) continue;
                    createTable = createTable.WithColumn(prop.Name).AsString(int.MaxValue).Nullable();
                }
                createTable.Do();
            }
        }

        protected void CreateIndex(Type type, string[] columns)
        {
            var attri = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attri == null) return;

            var tableName = type.Name;
            var indexName = $"IX_{tableName}_{string.Join("_", columns)}";

            if (!IndexExists(indexName))
            {
                var createIndex = Create.Index(indexName).OnTable(tableName);
                foreach (var col in columns)
                {
                    createIndex = createIndex.OnColumn(col).Ascending();
                }
                createIndex.Do();
            }
        }
    }
} 