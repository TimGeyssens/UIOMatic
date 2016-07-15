using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Example.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Example
{
    public class Startup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //Some old code to create db tables that don't exist yet
            //var db = applicationContext.DatabaseContext.Database;
            //if (!db.TableExist("TestWithDate"))
            //    db.CreateTable<TestWithDate>(false);


            //Use scaffolding object event to set default values...http://www.nibble.be/?p=497
            UIOMatic.Controllers.PetaPocoObjectController.ScaffoldingObject += PetaPocoObjectController_ScaffoldingObject;

            //Add additional where statements to the query in the building query event
            UIOMatic.Controllers.PetaPocoObjectController.BuildingQuery += PetaPocoObjectController_BuildingQuery1;

            //Full control over the query in the builder query event (so you can even overwrite), 
            UIOMatic.Controllers.PetaPocoObjectController.BuildedQuery += PetaPocoObjectController_BuildedQuery;
        }

        private void PetaPocoObjectController_ScaffoldingObject(object sender, UIOMatic.ObjectEventArgs e)
        {
            if (e.Object.GetType() == typeof(TestWithDate))
                ((TestWithDate)e.Object).TheDate = DateTime.Now;
        }

        private void PetaPocoObjectController_BuildingQuery1(object sender, UIOMatic.QueryEventArgs e)
        {
           
            if (e.CurrentType == typeof(TestWithDateLimit))
            {
                e.Query.Where("TheDate >= @0", DateTime.Now.AddDays(-1));

            }
        }

        private void PetaPocoObjectController_BuildedQuery(object sender, UIOMatic.QueryEventArgs e)
        {
            if (e.TableName == "Dogs")
            {

                e.Query = Umbraco.Core.Persistence.Sql.Builder
                    .Append("SELECT Dogs.Id, Dogs.Name, Dogs.IsCastrated, Dogs.OwnerId, People.Firstname + ' ' + People.Lastname as OwnerName")
                    .Append("FROM Dogs")
                    .Append("INNER JOIN People ON Dogs.OwnerId = People.Id")
                    .Append(string.IsNullOrEmpty(e.SearhTerm) ? "" : string.Format(
                        @"WHERE Dogs.Name like '%{0}%' 
                            or People.Firstname like '%{0}%' 
                            or People.Lastname like '%{0}%'", e.SearhTerm))
                    .Append("ORDER BY " + (string.IsNullOrEmpty(e.SortColumn) ? " Id desc" :  e.SortColumn + " " + e.SortOrder));


            }

            
        }
    }
}