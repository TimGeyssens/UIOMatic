//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using UIOMatic.Attributes;
//using UIOMatic.Enums;
//using UIOMatic.Interfaces;
//using Umbraco.Core.Persistence;
//using Umbraco.Core.Persistence.DatabaseAnnotations;

//namespace Example.Models
//{
//    [UIOMatic("Test", "icon-users", "icon-user", RenderType = UIOMaticRenderType.List )]
//    [TableName("Test")]
//    public class Test : IUIOMaticModel
//    {
//        [PrimaryKeyColumn]
//        public Guid Id { get; set; }

//        //[UIOMaticNameField]
//        public string Testing { get; set; }

//        public IEnumerable<Exception> Validate()
//        {
//            return new List<Exception>();
//        }

//        public override string ToString()
//        {
//            return Testing;
//        }
//    }
//}