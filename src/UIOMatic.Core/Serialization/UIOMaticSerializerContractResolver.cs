using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;


namespace UIOMatic.Serialization
{
    public class UIOMaticSerializerContractResolver : DefaultContractResolver
    {
     
        public UIOMaticSerializerContractResolver()
        {
          
        }
        
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (member.Name != "CurrentPage" &&
                member.Name != "ItemsPerPage" &&
                member.Name != "TotalPages" &&
                member.Name != "TotalItems" &&
                member.Name != "Items")
            {
                property.PropertyName = member.Name;
            }

            return property;
        }
    }
}
