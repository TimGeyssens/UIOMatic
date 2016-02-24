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
            var db = applicationContext.DatabaseContext.Database;
            if (!db.TableExist("TestWithDate"))
                db.CreateTable<TestWithDate>(false);

            UIOMatic.Controllers.PetaPocoObjectController.ScaffoldingObject += PetaPocoObjectController_ScaffoldingObject;

            UIOMatic.Controllers.PetaPocoObjectController.BuildingQuery += PetaPocoObjectController_BuildingQuery1;

            UIOMatic.Controllers.PetaPocoObjectController.BuildedQuery += PetaPocoObjectController_BuildingQuery;
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

        private void PetaPocoObjectController_BuildingQuery(object sender, UIOMatic.QueryEventArgs e)
        {
            if (e.TableName == "Dogs")
            {

                e.Query = Umbraco.Core.Persistence.Sql.Builder
                    .Append("SELECT Dogs.Id, Dogs.Name, Dogs.IsCastrated, Dogs.OwnerId, People.Firstname + ' ' + People.Lastname as OwnerName")
                    .Append("FROM Dogs")
                    .Append("INNER JOIN People ON Dogs.OwnerId = People.Id")
                    .Append("ORDER BY Dogs.Id desc");


            }

            
        }
    }
}