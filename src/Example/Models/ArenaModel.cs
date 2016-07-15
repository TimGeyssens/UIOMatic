using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UIOMatic.Attributes;
using UIOMatic.Interfaces;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Example.Models
{
    [UIOMatic("Arenor", "icon-users", "icon-user")]
    [TableName("tnmArena")]
    public class ArenaModel : IUIOMaticModel
    {
      

        [PrimaryKeyColumn(AutoIncrement = false)]
        [UIOMaticField("Arena", "Ange arena id", View = "number")]
        public int Id { get; set; }

        [UIOMaticField("Arenanamn", "Ange arenanamn, tex BillerudKorsnäs", View = "textfield")]
        public string ArenaName { get; set; }

        public override string ToString()
        {
            return ArenaName;
        }
        public IEnumerable<Exception> Validate()
        {
            var exs = new List<Exception>();

            return exs;
        }
    }
}