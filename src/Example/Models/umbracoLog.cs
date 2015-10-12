//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using UIOMatic.Atributes;
//using UIOMatic.Enums;
//using UIOMatic.Interfaces;
//using Umbraco.Core.Persistence;
//using Umbraco.Core.Persistence.DatabaseAnnotations;

//namespace Example.Models
//{
//    [TableName("umbracoLog")]
//    [UIOMatic("Log", "icon-users", "icon-user", ConnectionStringName = "thirdpartyDbDSN",RenderType = UIOMaticRenderType.List)]
//    public class UmbracoLog: IUIOMaticModel
//    {
//        [Column("id")]
//        [PrimaryKeyColumn(AutoIncrement = true)]
//        public int Id { get; set; }

//         [Column("userId")]
//        public int UserId { get; set; }

      
//        public int NodeId { get; set; }

//        public DateTime Datestamp { get; set; }

//         [Column("logHeader")]
//        public string LogHeader { get; set; }

//         [Column("logComment")]
//        public string LogComment { get; set; }
//        public IEnumerable<Exception> Validate()
//        {
//            return new List<Exception>();
//        }
//    }
//}