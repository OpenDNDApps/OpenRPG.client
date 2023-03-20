using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace VGDevs
{
    public class LocalPersistenceContractResolver : DefaultContractResolver
    {
        private const BindingFlags kBindingFlags = BindingFlags.Public
                                                   | BindingFlags.NonPublic
                                                   | BindingFlags.Instance
                                                   | BindingFlags.DeclaredOnly;
        
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            List<MemberInfo> members = new List<MemberInfo>();

            members.AddRange(type.GetFields(kBindingFlags));
            members.AddRange(type.GetProperties(kBindingFlags));

            if (type.BaseType != null)
            {
                members.AddRange(type.BaseType.GetFields(kBindingFlags));
                members.AddRange(type.BaseType.GetProperties(kBindingFlags));
            }

            members = members.Where(memberInfo => Attribute.IsDefined(memberInfo, typeof(SerializeField)) || 
                                                        Attribute.IsDefined(memberInfo, typeof(JsonPropertyAttribute)) || 
                                                        (memberInfo as FieldInfo)?.IsPublic == true).ToList();
            
            List<JsonProperty> properties = new List<JsonProperty>();

            foreach (MemberInfo member in members)
            {
                properties.Add(GetProperty(member, memberSerialization));
            }
            
            return properties;
        }

        private JsonProperty GetProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = CreateProperty(member, memberSerialization);
            property.Writable = true;
            property.Readable = true;
            return property;
        }
    }
}