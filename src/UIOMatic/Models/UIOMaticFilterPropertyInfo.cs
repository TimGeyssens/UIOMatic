using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace UIOMatic.Models
{
    public class UIOMaticFilterPropertyInfo
    {
        public string Key { get; set; }
        public string Name { get; set; }

        public string Tab { get; set; }

        public string Description { get; set; }

        public string View { get; set; }

        public string Type { get; set; }

        public JObject Config { get; set; }

        public string OperatorID { get; set; }

        public string Value { get; set; }

        public string ToValue { get; set; }
    }
}