using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UIOMatic.Atributes;
using UIOMatic.Enums;
using UIOMatic.Interfaces;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Example.Models
{
    [UIOMatic("DateTest", "icon-users", "icon-user")]
    [TableName("DateTest")]
    public class DateTest: IUIOMaticModel
    {
        [UIOMaticIgnoreField]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        public DateTime TheDate { get; set; }

        public IEnumerable<Exception> Validate()
        {
            return new List<Exception>();
        }
    }
}