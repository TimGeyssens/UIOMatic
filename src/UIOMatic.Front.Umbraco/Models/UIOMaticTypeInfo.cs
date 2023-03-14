using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UIOMatic.Enums;
using Umbraco.Cms.Core.Models.ContentEditing;

namespace UIOMatic.Front.Umbraco.Models
{
    public class UIOMaticTypeInfo: UIOMatic.Models.UIOMaticTypeInfo
    {

        [JsonProperty("apps")]
        public IEnumerable<ContentApp> Apps { get; set; }


    }
}