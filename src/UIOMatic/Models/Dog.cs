using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UIOMatic.Atributes;
using UIOMatic.Interfaces;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace UIOMatic.Models
{
    [UIOMatic("Dogs", "icon-users", "icon-user")]
    [TableName("Dogs")]
    public class Dog : IUIOMaticModel
    {
        public Dog() { }

        [UIOMaticIgnoreField]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }


        public string Name { get; set; }


        public string OwnerName { get; set; }


        [Ignore]
        public string UmbracoTreeNodeName
        {
            get { return Name; }
        }


        public IEnumerable<Exception> Validate()
        {
            var exs = new List<Exception>();

            if (string.IsNullOrEmpty(Name))
                exs.Add(new Exception("Please provide a value for name"));

            if (string.IsNullOrEmpty(OwnerName))
                exs.Add(new Exception("Please provide a value for owner name"));


            return exs;
        }
    }
}