using System;
using System.Collections.Generic;
using UIOMatic.Attributes;
using UIOMatic.Enums;
using UIOMatic.Interfaces;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Example.Models
{
    [UIOMatic("Dogs", "icon-users", "icon-user", RenderType = UIOMaticRenderType.List)]
    [TableName("Dogs")]
    public class Dog : IUIOMaticModel
    {
        public Dog() { }

        [UIOMaticIgnoreField]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }


        public string Name { get; set; }

        [UIOMaticField("Is castrated", "Has the dog been castrated")]
        public bool IsCastrated { get; set; }


        [UIOMaticField("Owner", "Select the owner of the dog", View = "dropdown", Tab = "Owner",
            Config = "{'typeName': 'Example.Models.Person, Example', 'valueColumn': 'Id', 'textColumn' : 'FirstName'}")]
        [UIOMaticIgnoreFromListView]
        public int OwnerId { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerable<Exception> Validate()
        {
            var exs = new List<Exception>();

            if (string.IsNullOrEmpty(Name))
                exs.Add(new Exception("Please provide a value for name"));

            if (OwnerId == 0)
                exs.Add(new Exception("Please select an owner"));


            return exs;
        }
    }
}