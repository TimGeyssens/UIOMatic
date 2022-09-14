using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UIOMatic.Extensions;

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
