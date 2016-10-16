using System;
using System.Collections.Generic;

using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Example.Models
{
    [UIOMatic(name: "TestWithDateTimeOffset", folderIcon: "icon-users", itemIcon: "icon-user", RenderType = UIOMatic.Enums.UIOMaticRenderType.List, ReadOnly = false)]
    [TableName("TestWithDateTimeOffset")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class TestWithDateTimeOffset : IUIOMaticModel
    {
        [UIOMaticIgnoreField]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [UIOMaticField("Firstname", "Enter your firstname")]
        public string FirstName { get; set; }

        [UIOMaticField("Lastname", "Enter your lastname")]
        public string LastName { get; set; }

        [UIOMaticField("TheDateTimeOffset", "select a date")]
        public DateTimeOffset TheDateTimeOffset { get; set; }

        public IEnumerable<Exception> Validate()
        {
            return new List<Exception>();
        }
    }
}