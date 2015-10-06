using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using UIOMatic.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace UIOMatic
{
    public class Startup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var db = applicationContext.DatabaseContext.Database;
            if (!db.TableExist("People"))
                db.CreateTable<Person>(false);

            if (!db.TableExist("Dogs"))
                db.CreateTable<Dog>(false);

            //foreach (var tableType in Helper.GetTypesWithUIOMaticAttribute())
            //{


            //    var tableName = (TableNameAttribute)Attribute.GetCustomAttribute(tableType, typeof(TableNameAttribute));
            //    //Check if the DB table does NOT exist
            //    if (!db.TableExist(tableName.Value))
            //    {
            //        //Create DB table - and set overwrite to false
                    
            //        //db.CreateTable<Dog>(false);

            //        //Todo do this with reflection, currenty failing
            //        object[] args = { false };
            //        db.GetType().GetMethod("CreateTable").MakeGenericMethod(tableType).Invoke(db, args);
            //    }
            //}
           
        }

        
    }
}