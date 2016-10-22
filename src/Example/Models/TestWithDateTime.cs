using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using UIOMatic.Attributes;
using UIOMatic.Enums;
using UIOMatic.Interfaces;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Example.Models
{
    [UIOMatic("TestWithDateTime", "icon-users", "icon-user", RenderType = UIOMaticRenderType.List,
        SortColumn = "TheDateTime", SortOrder = "desc")]
    [TableName("TestWithDateTime")]
    public class TestWithDateTime : IUIOMaticModel
    {
        public TestWithDateTime()
        {

        }


        [UIOMaticIgnoreField]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [UIOMaticField("Firstname", "Enter your firstname")]
        public string FirstName { get; set; }

        [UIOMaticField("Lastname", "Enter your lastname")]
        public string LastName { get; set; }

        [UIOMaticField("TheDateTime", "Select a date time")]
        public DateTime TheDateTime { get; set; }

        public IEnumerable<Exception> Validate()
        {
          
            return new List<Exception>();
        }
    }
}