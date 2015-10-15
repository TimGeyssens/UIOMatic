using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UIOMatic.Atributes;
using UIOMatic.Enums;
using UIOMatic.Interfaces;
using Umbraco.Core.Persistence;

namespace Example.Models
{
    [TableName("Shippers")]


    [PrimaryKey("Shipper ID")]



    [UIOMatic("Shippers", "icon-box-open", "icon-box-open", RenderType = UIOMaticRenderType.List)]
    public class Shipper : IUIOMaticModel
    {





        [UIOMaticIgnoreField]

        [Column("Shipper ID")]

        public int Shipper_ID { get; set; }





        [Column("Company Name")]

        public string Company_Name { get; set; }




        public IEnumerable<Exception> Validate()
        {
            //add validation rules here
            return new List<Exception>();
        }
    }
}