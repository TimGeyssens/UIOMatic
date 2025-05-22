using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using UIOMatic.Interfaces;
using UIOMatic.Front.API.Models;
using UIOMatic.Models;
using UIOMatic.Attributes;
using UIOMatic.Core.Repositories;
using UIOMatic.Services;
using Newtonsoft.Json;

namespace UIOMatic.Front.API.Repositories
{
    public class FileContentRepository : UIOMaticRepositoryBase
    {
        private readonly IWebHostEnvironment _environment;
        private string _contentPath;
        private string _filePattern;
        private Type _entityType;
        private int _nextId;
        private readonly UIOMaticObjectService _uioMaticObjectService;
        private readonly UIOMaticAttribute _attr;
        private readonly UIOMaticTypeInfo _typeInfo;
        private readonly string _filePath;

        public FileContentRepository() : this(null)
        {
        }

        public FileContentRepository(IWebHostEnvironment environment)
        {
            _environment = environment;
            _contentPath = System.IO.Path.Combine(environment?.ContentRootPath ?? Directory.GetCurrentDirectory(), "content");
            _filePattern = "*.json";
            _entityType = typeof(Content);
            
            // Ensure the content directory exists
            if (!Directory.Exists(_contentPath))
            {
                Directory.CreateDirectory(_contentPath);
            }

            // Initialize or load the next ID
            if (File.Exists(System.IO.Path.Combine(_contentPath, ".nextid")))
            {
                _nextId = int.Parse(File.ReadAllText(System.IO.Path.Combine(_contentPath, ".nextid")));
            }
            else
            {
                _nextId = 1;
                File.WriteAllText(System.IO.Path.Combine(_contentPath, ".nextid"), _nextId.ToString());
            }
        }

        public FileContentRepository(
            UIOMaticAttribute attr,
            UIOMaticTypeInfo typeInfo,
            UIOMaticObjectService uioMaticObjectService)
        {
            _attr = attr;
            _typeInfo = typeInfo;
            _uioMaticObjectService = uioMaticObjectService;
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "UIOMatic", $"{typeInfo.Type.Name}.json");
        }

        private async Task SaveNextId()
        {
            await File.WriteAllTextAsync(System.IO.Path.Combine(_contentPath, ".nextid"), _nextId.ToString());
        }

        public override void Initialize(object[] parameters)
        {
            if (parameters == null || parameters.Length < 3)
                throw new ArgumentException("FileContentRepository requires content path, file pattern, and entity type parameters");

            _contentPath = parameters[0] as string;
            _filePattern = parameters[1] as string;
            _entityType = parameters[2] as Type;

            if (string.IsNullOrEmpty(_contentPath) || string.IsNullOrEmpty(_filePattern) || _entityType == null)
                throw new ArgumentException("Invalid parameters for FileContentRepository");

            // Ensure the content directory exists
            if (!Directory.Exists(_contentPath))
            {
                Directory.CreateDirectory(_contentPath);
            }
        }

        private string GetFilePath(string id) => System.IO.Path.Combine(_contentPath, $"{id}.json");

        public override IEnumerable<object> GetAll(string sortColumn, string sortOrder)
        {
            var files = Directory.GetFiles(_contentPath, _filePattern);
            var items = new List<object>();

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var item = JsonSerializer.Deserialize(json, _entityType);
                items.Add(item);
            }

            // Apply sorting if specified
            if (!string.IsNullOrEmpty(sortColumn))
            {
                var property = _entityType.GetProperty(sortColumn);
                if (property != null)
                {
                    items = sortOrder?.ToLower() == "desc" 
                        ? items.OrderByDescending(x => property.GetValue(x)).ToList()
                        : items.OrderBy(x => property.GetValue(x)).ToList();
                }
            }

            return items;
        }

        public override object GetById(string id)
        {
            Console.WriteLine($"Getting item by ID: {id}");
            var filePath = GetFilePath(id);
            Console.WriteLine($"Looking for file at: {filePath}");
            
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return null;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                Console.WriteLine($"File content: {json}");
                return JsonSerializer.Deserialize(json, _entityType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
                return null;
            }
        }

        public override object Create(object entity)
        {
            // Get the next available ID
            var id = _nextId++;
            SaveNextId().Wait(); // Save the new ID

            // Set the Id property if it exists
            var idProperty = _entityType.GetProperty("Id");
            if (idProperty != null)
            {
                idProperty.SetValue(entity, id);
            }

            var filePath = GetFilePath(id.ToString());
            var json = JsonSerializer.Serialize(entity);
            File.WriteAllText(filePath, json);

            return entity;
        }

        public override object Update(object entity)
        {
            var idProperty = _entityType.GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Entity must have an Id property");

            var id = idProperty.GetValue(entity)?.ToString();
            if (string.IsNullOrEmpty(id))
                throw new InvalidOperationException("Entity Id cannot be null or empty");

            Console.WriteLine($"Updating item with ID: {id}");
            var filePath = GetFilePath(id);
            Console.WriteLine($"File path: {filePath}");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                throw new FileNotFoundException($"Entity with id {id} not found");
            }

            try
            {
                var json = JsonSerializer.Serialize(entity);
                Console.WriteLine($"Writing content: {json}");
                File.WriteAllText(filePath, json);
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating file {filePath}: {ex.Message}");
                throw;
            }
        }

        public override void Delete(string[] ids)
        {
            foreach (var id in ids)
            {
                var filePath = GetFilePath(id);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        public override IEnumerable<ValidationResult> Validate(object entity)
        {
            var context = new ValidationContext(entity);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(entity, context, results, true);
            return results;
        }

        public override object Get(string id)
        {
            Console.WriteLine($"Get called with ID: {id}");
            return GetById(id);
        }

        public override long GetTotalRecordCount()
        {
            return Directory.GetFiles(_contentPath, _filePattern).Length;
        }

        public override UIOMaticPagedResult GetPaged(int pageNumber, int itemsPerPage, string searchTerm, IDictionary<string, string> filters, string sortColumn, string sortOrder)
        {
            var allItems = new List<object>();
            var files = Directory.GetFiles(_contentPath, _filePattern);
            
            Console.WriteLine($"Found {files.Length} files in {_contentPath} matching pattern {_filePattern}");

            foreach (var file in files)
            {
                try
                {
                    Console.WriteLine($"Reading file: {file}");
                    var json = File.ReadAllText(file);
                    if (!string.IsNullOrEmpty(json))
                    {
                        Console.WriteLine($"File content: {json}");
                        var item = JsonSerializer.Deserialize(json, _entityType);
                        if (item != null)
                        {
                            allItems.Add(item);
                            Console.WriteLine($"Successfully deserialized item with ID: {item.GetType().GetProperty("Id")?.GetValue(item)}");
                        }
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing file {file}: {ex.Message}");
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error processing file {file}: {ex.Message}");
                    continue;
                }
            }

            Console.WriteLine($"Total items found: {allItems.Count}");

            var totalItems = allItems.Count;

            // Apply search if specified
            if (!string.IsNullOrEmpty(searchTerm))
            {
                allItems = allItems.Where(item =>
                {
                    return _entityType.GetProperties()
                        .Any(p => p.GetValue(item)?.ToString()?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false);
                }).ToList();
            }

            // Apply filters if specified
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    var property = _entityType.GetProperty(filter.Key);
                    if (property != null)
                    {
                        allItems = allItems.Where(item =>
                            property.GetValue(item)?.ToString() == filter.Value).ToList();
                    }
                }
            }

            // Apply sorting if specified
            if (!string.IsNullOrEmpty(sortColumn))
            {
                var property = _entityType.GetProperty(sortColumn);
                if (property != null)
                {
                    allItems = sortOrder?.ToLower() == "desc" 
                        ? allItems.OrderByDescending(x => property.GetValue(x)).ToList()
                        : allItems.OrderBy(x => property.GetValue(x)).ToList();
                }
            }

            // Apply paging
            var pagedItems = allItems
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            return new UIOMaticPagedResult
            {
                Items = pagedItems,
                TotalItems = totalItems,
                CurrentPage = pageNumber,
                ItemsPerPage = itemsPerPage,
                TotalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage)
            };
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
        }

        public override async Task<IEnumerable<object>> GetAllAsync(string sortColumn = "", string sortOrder = "")
        {
            var items = await ReadFromFile();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                items = sortOrder == "asc"
                    ? items.OrderBy(x => x[sortColumn])
                    : items.OrderByDescending(x => x[sortColumn]);
            }
            return items.Select(x => _uioMaticObjectService.MapToObject(_typeInfo.Type, x));
        }

        public override async Task<object> GetAsync(string id)
        {
            var items = await ReadFromFile();
            var item = items.FirstOrDefault(x => x["Id"].ToString() == id);
            return item != null ? _uioMaticObjectService.MapToObject(_typeInfo.Type, item) : null;
        }

        public override async Task<object> CreateAsync(object entity)
        {
            var items = await ReadFromFile();
            var doc = _uioMaticObjectService.MapToDocument(entity);
            doc["Id"] = Guid.NewGuid().ToString();
            items.Add(doc);
            await WriteToFile(items);
            return entity;
        }

        public override async Task<object> UpdateAsync(object entity)
        {
            var items = await ReadFromFile();
            var doc = _uioMaticObjectService.MapToDocument(entity);
            var index = items.FindIndex(x => x["Id"].ToString() == doc["Id"].ToString());
            if (index >= 0)
            {
                items[index] = doc;
                await WriteToFile(items);
            }
            return entity;
        }

        public override async Task DeleteAsync(string[] ids)
        {
            var items = await ReadFromFile();
            items.RemoveAll(x => ids.Contains(x["Id"].ToString()));
            await WriteToFile(items);
        }

        public override async Task<object> ValidateAsync(object entity)
        {
            return await Task.FromResult(_uioMaticObjectService.Validate(_typeInfo.Type, entity));
        }

        public override async Task<long> GetTotalRecordCountAsync()
        {
            var items = await ReadFromFile();
            return items.Count;
        }

        public override async Task<UIOMaticPagedResult> GetPagedAsync(
            int pageNumber,
            int itemsPerPage,
            string searchTerm = "",
            IDictionary<string, string> filters = null,
            string sortColumn = "",
            string sortOrder = "")
        {
            var items = await ReadFromFile();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                items = sortOrder == "asc"
                    ? items.OrderBy(x => x[sortColumn])
                    : items.OrderByDescending(x => x[sortColumn]);
            }
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    items = items.Where(x => x[filter.Key].ToString() == filter.Value);
                }
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                items = items.Where(x => x.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }
            var totalItems = items.Count();
            var results = items.Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage)
                .Select(x => _uioMaticObjectService.MapToObject(_typeInfo.Type, x));
            return new UIOMaticPagedResult
            {
                CurrentPage = pageNumber,
                ItemsPerPage = itemsPerPage,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage),
                Items = results
            };
        }

        private async Task<List<Dictionary<string, object>>> ReadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Dictionary<string, object>>();
            }

            var json = await File.ReadAllTextAsync(_filePath);
            return System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json) ?? new List<Dictionary<string, object>>();
        }

        private async Task WriteToFile(List<Dictionary<string, object>> items)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(items);
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
} 