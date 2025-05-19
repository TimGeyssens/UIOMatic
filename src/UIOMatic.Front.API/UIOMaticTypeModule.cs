using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Descriptors.Definitions;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Text.Json;
using UIOMatic.Attributes;
using UIOMatic.Front.API.Extensions;
using UIOMatic.Interfaces;
using UIOMatic.Services;

namespace UIOMatic.Front.API
{
    public class UIOMaticTypeModule : ITypeModule
    {
        public event EventHandler<EventArgs> TypesChanged; 

        private readonly IUIOMaticHelper _helper;
        private readonly IUIOMaticObjectService _service;
        public UIOMaticTypeModule(IUIOMaticHelper helper, IUIOMaticObjectService uioMaticObjectService)
        {
            _helper = helper;
            _service = uioMaticObjectService;
        }

        public ValueTask<IReadOnlyCollection<ITypeSystemMember>> CreateTypesAsync(IDescriptorContext context, CancellationToken cancellationToken)
        {
            var types = new List<ITypeSystemMember>();
            var queryType = new ObjectTypeDefinition("Query");
            var mutationType = new ObjectTypeDefinition("Mutation");
            foreach (var type in _helper.GetUIOMaticTypes())
            {

                var info = _service.GetTypeInfo(type, true);

                var schemaNamePascalCase = info.DisplayNameSingular!.ToPascalCase();
                var schemaNamePluralCamelCase = info.DisplayNamePlural!.ToCamelCase();

                var objectTypeDefinition = new ObjectTypeDefinition(schemaNamePascalCase);

         

                foreach (var propertyInfo in type.GetProperties())
                {
                    var fld = new ObjectFieldDefinition(propertyInfo.Name.ToCamelCase(),
                        type: TypeReference.Parse("String?"),
                        pureResolver: ctx =>
                            ctx.Parent<IDictionary<string, object>>().FirstOrDefault(v => string.Equals(v.Key, propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase)).Value?.ToString());
                          

                    objectTypeDefinition.Fields.Add(fld);
                }

                queryType.Fields.Add(new ObjectFieldDefinition(schemaNamePluralCamelCase)
                {
                    Type = TypeReference.Parse($"[{schemaNamePascalCase}]"),
                    Resolver = async (ctx) =>
                    {
                        var items = _service.GetAll(type,"","");
                        return items;
                    },
                    
                    
                }
                .ToDescriptor(context)
                .ToDefinition());

                queryType
                 .ToDescriptor(context)
                 .GetEntityById(type, schemaNamePascalCase)
                 .ToDefinition();

                mutationType.ToDescriptor(context)
                  .DeleteEntityById(type, schemaNamePascalCase)
                  .ToDefinition();

                types.Add(ObjectType.CreateUnsafe(objectTypeDefinition));
            }
            types.Add(ObjectType.CreateUnsafe(queryType));
            types.Add(ObjectType.CreateUnsafe(mutationType));
            return new ValueTask<IReadOnlyCollection<ITypeSystemMember>>(types);
        }



    }
        public static class QueryExtensionMethods
        {
            public static IObjectTypeDescriptor GetEntityById(
                this IObjectTypeDescriptor descriptor,
                Type type, string name) 
            {

                descriptor.Field($"{name.ToCamelCase()}ById")
                    .Argument("id", a => a.Type<NonNullType<StringType>>())
                    .Resolve(x =>
                    {
                        var service = x.Service<IUIOMaticObjectService>();
                        var helper = x.Service<IUIOMaticHelper>();
                        var id = x.ArgumentValue<string>("id");

                        return service.GetById(type, id);
                    })
                    .Type($"[{name}]");

            return descriptor;
            }

        public static IObjectTypeDescriptor DeleteEntityById(
               this IObjectTypeDescriptor descriptor,
               Type type, string name)
        {

            descriptor.Field($"delete{name}ById")
                .Argument("id", a => a.Type<NonNullType<StringType>>())
                .Resolve(x =>
                {
                    var service = x.Service<IUIOMaticObjectService>();
                    var helper = x.Service<IUIOMaticHelper>();
                    var id = x.ArgumentValue<string>("id");

                    return service.DeleteByIds(type, new string[] { id });
                })
                .Type($"[{name}]");

            return descriptor;
        }
    }

}
