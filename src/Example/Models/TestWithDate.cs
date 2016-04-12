using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using UIOMatic.Core.Attributes;
using UIOMatic.Core.Enums;
using UIOMatic.Core.Interfaces;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Example.Models
{
    [UIOMatic("TestWithDate", "icon-users", "icon-user", RenderType = UIOMaticRenderType.List,
        SortColumn = "TheDate", SortOrder = "desc")]
    [TableName("TestWithDate")]
    public class TestWithDate : IUIOMaticModel
    {
        public TestWithDate()
        {

        }


        [UIOMaticIgnoreField]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [UIOMaticField("Firstname", "Enter your firstname")]
        public string FirstName { get; set; }

        [UIOMaticField("Lastname", "Enter your lastname")]
        public string LastName { get; set; }

        [UIOMaticField("TheDate", "Select a date")]
        public DateTime TheDate { get; set; }

        public IEnumerable<Exception> Validate()
        {
          
            return new List<Exception>();
        }
    }
}