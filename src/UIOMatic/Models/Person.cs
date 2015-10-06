//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Web;
//using UIOMatic.Atributes;
//using UIOMatic.Interfaces;
//using Umbraco.Core.Persistence;
//using Umbraco.Core.Persistence.DatabaseAnnotations;

//namespace UIOMatic.Models
//{
//    [UIOMaticAttribute("People","icon-users","icon-user")]
//    [TableName("People")]
//    public class Person: IUIOMaticModel
//    {
//        public Person() { }

//        [UIOMaticIgnoreField]
//        [PrimaryKeyColumn(AutoIncrement = true)]
//        public int Id { get; set; }

//        [UIOMaticField("First name","Enter the persons first name")]
//        public string FirstName { get; set; }

//        [UIOMaticField("Last name", "Enter the persons last name")]
//        public string LastName { get; set; }

//        [UIOMaticField("Picture", "Select a picture", View = "file")]
//        public string Picture { get; set; }

//        [Ignore]
//        public string UmbracoTreeNodeName
//        {
//            get { return FirstName + " " + LastName; }
//        }


//        public IEnumerable<Exception> Validate()
//        {
//            var exs = new List<Exception>();

//            if(string.IsNullOrEmpty(FirstName))
//                exs.Add(new Exception("Please provide a value for first name"));

//            if (string.IsNullOrEmpty(LastName))
//                exs.Add(new Exception("Please provide a value for last name"));


//            return exs;
//        }
//    }
//}