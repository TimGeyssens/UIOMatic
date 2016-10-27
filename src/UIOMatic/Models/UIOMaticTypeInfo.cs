using System;
using Newtonsoft.Json;
using UIOMatic.Enums;

namespace UIOMatic.Models
{
    public class UIOMaticTypeInfo
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("displayNamePlural")]
        public string DisplayNamePlural { get; set; }

        [JsonProperty("displayNameSingular")]
        public string DisplayNameSingular { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tableName")]
        public string TableName { get; set; }

        [JsonProperty("primaryKeyColumnName")]
        public string PrimaryKeyColumnName { get; set; }

        [JsonProperty("autoIncrementPrimaryKey")]
        public bool AutoIncrementPrimaryKey { get; set; }

        [JsonProperty("renderType")]
        public UIOMaticRenderType RenderType { get; set; }

        [JsonProperty("rawProperties")]
        public UIOMaticPropertyInfo[] RawProperties { get; set; }

        [JsonProperty("editableProperties")]
        public UIOMaticEditablePropertyInfo[] EditableProperties { get; set; }

        [JsonProperty("listViewProperties")]
        public UIOMaticViewablePropertyInfo[] ListViewProperties { get; set; }

        [JsonProperty("listViewFilterProperties")]
        public UIOMaticFilterPropertyInfo[] ListViewFilterProperties { get; set; }

        [JsonProperty("nameFieldKey")]
        public string NameFieldKey { get; set; }

        [JsonProperty("readOnly")]
        public bool ReadOnly { get; set; }

        [JsonProperty("path")]
        public string[] Path { get; set; }

        [JsonProperty("type")]
        public Type Type { get; set; }

        [JsonProperty("listActions")]
        public UIOMaticActionInfo[] ListActions { get; set; }
    }
}