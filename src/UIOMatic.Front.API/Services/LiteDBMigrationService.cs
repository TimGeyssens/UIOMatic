using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.Extensions.Logging;
using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using UIOMatic.Front.API.Repositories;

namespace UIOMatic.Front.API.Services
{
    public class LiteDBMigrationService
    {
        private readonly LiteDatabase _db;
        private readonly ILogger<LiteDBMigrationService> _logger;
        private readonly string _migrationCollection = "_migrations";

        public LiteDBMigrationService(LiteDatabase db, ILogger<LiteDBMigrationService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task InitializeDatabase()
        {
            try
            {
                // Ensure migrations collection exists
                if (!_db.CollectionExists(_migrationCollection))
                {
                    _db.GetCollection(_migrationCollection);
                }

                // Get all types with UIOMatic attribute that use LiteDBRepository
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => 
                    {
                        var attr = t.GetCustomAttribute<UIOMaticAttribute>();
                        return attr != null && attr.RepositoryType == typeof(LiteDBRepository);
                    })
                    .ToList();

                _logger.LogInformation($"Found {types.Count} models using LiteDBRepository");

                foreach (var type in types)
                {
                    await EnsureCollectionExists(type);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing database");
                throw;
            }
        }

        private async Task EnsureCollectionExists(Type type)
        {
            var attribute = type.GetCustomAttribute<UIOMaticAttribute>();
            if (attribute == null) return;

            var collectionName = type.Name;
            _logger.LogInformation($"Processing collection: {collectionName} for type: {type.Name}");
            var collection = _db.GetCollection(collectionName);

            // Get existing schema
            var existingSchema = GetExistingSchema(collection);
            var currentSchema = GetCurrentSchema(type);

            // Compare schemas and update if needed
            if (existingSchema == null)
            {
                // New collection, create with schema
                _logger.LogInformation($"Creating new collection {collectionName} with schema");
                await CreateCollectionWithSchema(collection, currentSchema);
            }
            else
            {
                // Existing collection, check for changes
                var changes = CompareSchemas(existingSchema, currentSchema);
                if (changes.Any())
                {
                    _logger.LogInformation($"Updating schema for collection {collectionName}");
                    await UpdateCollectionSchema(collection, changes);
                }
            }
        }

        private Dictionary<string, string> GetExistingSchema(ILiteCollection<BsonDocument> collection)
        {
            var firstDoc = collection.FindOne(Query.All());
            if (firstDoc == null) return null;

            return firstDoc.Keys.ToDictionary(k => k, k => firstDoc[k].Type.ToString());
        }

        private Dictionary<string, string> GetCurrentSchema(Type type)
        {
            return type.GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .ToDictionary(
                    p => p.Name,
                    p => GetPropertyType(p.PropertyType)
                );
        }

        private string GetPropertyType(Type type)
        {
            if (type == typeof(string)) return "String";
            if (type == typeof(int) || type == typeof(long)) return "Int32";
            if (type == typeof(double) || type == typeof(float)) return "Double";
            if (type == typeof(bool)) return "Boolean";
            if (type == typeof(DateTime)) return "DateTime";
            if (type == typeof(Guid)) return "Guid";
            if (type.IsEnum) return "Int32";
            return "String"; // Default to string for complex types
        }

        private List<SchemaChange> CompareSchemas(Dictionary<string, string> existing, Dictionary<string, string> current)
        {
            var changes = new List<SchemaChange>();

            // Check for removed properties
            foreach (var existingProp in existing.Keys)
            {
                if (!current.ContainsKey(existingProp))
                {
                    changes.Add(new SchemaChange
                    {
                        PropertyName = existingProp,
                        ChangeType = ChangeType.Removed
                    });
                }
            }

            // Check for added or modified properties
            foreach (var currentProp in current.Keys)
            {
                if (!existing.ContainsKey(currentProp))
                {
                    changes.Add(new SchemaChange
                    {
                        PropertyName = currentProp,
                        ChangeType = ChangeType.Added,
                        NewType = current[currentProp]
                    });
                }
                else if (existing[currentProp] != current[currentProp])
                {
                    changes.Add(new SchemaChange
                    {
                        PropertyName = currentProp,
                        ChangeType = ChangeType.Modified,
                        OldType = existing[currentProp],
                        NewType = current[currentProp]
                    });
                }
            }

            return changes;
        }

        private async Task CreateCollectionWithSchema(ILiteCollection<BsonDocument> collection, Dictionary<string, string> schema)
        {
            // Create an empty document with the schema
            var schemaDoc = new BsonDocument();
            foreach (var prop in schema)
            {
                schemaDoc[prop.Key] = BsonValue.Null;
            }
            collection.Insert(schemaDoc);
        }

        private async Task UpdateCollectionSchema(ILiteCollection<BsonDocument> collection, List<SchemaChange> changes)
        {
            foreach (var change in changes)
            {
                switch (change.ChangeType)
                {
                    case ChangeType.Added:
                        // Add new property to all documents
                        collection.UpdateMany(
                            BsonExpression.Create("true"),
                            BsonExpression.Create($"$.{change.PropertyName} = null")
                        );
                        break;

                    case ChangeType.Removed:
                        // Remove property from all documents
                        collection.UpdateMany(
                            BsonExpression.Create("true"),
                            BsonExpression.Create($"unset($.{change.PropertyName})")
                        );
                        break;

                    case ChangeType.Modified:
                        // Convert property type in all documents
                        collection.UpdateMany(
                            BsonExpression.Create("true"),
                            BsonExpression.Create($"$.{change.PropertyName} = convert($.{change.PropertyName}, {change.NewType})")
                        );
                        break;
                }
            }
        }
    }

    public class SchemaChange
    {
        public string PropertyName { get; set; }
        public ChangeType ChangeType { get; set; }
        public string OldType { get; set; }
        public string NewType { get; set; }
    }

    public enum ChangeType
    {
        Added,
        Removed,
        Modified
    }
} 