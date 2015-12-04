using System;
using System.Collections.Generic;
using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Example.Models
{

    [UIOMatic("People", "icon-users", "icon-user",SortColumn="FirstName")]
    [TableName("People")]
    public class Person : IUIOMaticModel
    {
        public Person() { }

        [UIOMaticIgnoreField]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [UIOMaticField("Firstname","Enter your firstname")]
        public string FirstName { get; set; }

        [UIOMaticField("Lastname", "Enter your lastname")]
        public string LastName { get; set; }

        [UIOMaticField("Picture", "Please select a picture",View ="file")]
        public string Picture { get; set; }

        [Ignore]
        [UIOMaticField("Dogs", "Manage your pets", View ="list",
            Config = "{'typeName': 'Example.Models.Dog, Example', 'foreignKeyColumn' : 'OwnerId'}")]
        public IEnumerable<Dog> Dogs { get; set; } 

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        public IEnumerable<Exception> Validate()
        {
            var exs = new List<Exception>();

            if (string.IsNullOrEmpty(FirstName))
                exs.Add(new Exception("Please provide a value for first name"));

            if (string.IsNullOrEmpty(LastName))
                exs.Add(new Exception("Please provide a value for last name"));


            return exs;
        }
    }
}