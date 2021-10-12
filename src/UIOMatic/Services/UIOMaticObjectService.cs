using System;
using UIOMatic;
using UIOMatic.Interfaces;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Hosting;

namespace UIOMatic.Services
{
    public class UIOMaticObjectService 
    {
        public static event EventHandler<QueryEventArgs> BuildingQuery;
        public static event EventHandler<QueryEventArgs> BuiltQuery;

        public static event EventHandler<ObjectEventArgs> ScaffoldingObject;

        public static event EventHandler<ObjectEventArgs> UpdatingObject;
        public static event EventHandler<ObjectEventArgs> UpdatedObject;

        public static event EventHandler<ObjectEventArgs> CreatingObject;
        public static event EventHandler<ObjectEventArgs> CreatedObject;

        public static event EventHandler<DeleteEventArgs> DeletingObjects;
        public static event EventHandler<DeleteEventArgs> DeletedObjects;

        private static Lazy<IUIOMaticObjectService> _instance = 
            new Lazy<IUIOMaticObjectService>(() =>

             new NPocoObjectService(ServiceLocator.ServiceProvider.GetService<AppCaches>(),
                 ServiceLocator.ServiceProvider.GetService<IHostingEnvironment>(),
                 ServiceLocator.ServiceProvider.GetService<IUIOMaticHelper>())
            //(IUIOMaticObjectService)Activator.CreateInstance(ServiceLocator.ServiceProvider.GetService<IUIOMaticConfiguration>().DefaultObjectServiceType ?? typeof(UIOMaticConfiguration), null));
            );
       
                          
                
       
        private UIOMaticObjectService()
        { }

        public static IUIOMaticObjectService Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public static void OnBuildingQuery(QueryEventArgs args)
        {
            if (BuildingQuery != null)
            {
                BuildingQuery(Instance, args);
            }
        }

        public static void OnBuiltQuery(QueryEventArgs args)
        {
            if (BuiltQuery != null)
            {
                BuiltQuery(Instance, args);
            }
        }

        public static void OnScaffoldingObject(ObjectEventArgs args)
        {
            if (ScaffoldingObject != null)
            {
                ScaffoldingObject(Instance, args);
            }
        }

        public static void OnUpdatingObject(ObjectEventArgs args)
        {
            if (UpdatingObject != null)
            {
                UpdatingObject(Instance, args);
            }
        }

        public static void OnUpdatedObject(ObjectEventArgs args)
        {
            if (UpdatedObject != null)
            {
                UpdatedObject(Instance, args);
            }
        }

        public static void OnCreatingObject(ObjectEventArgs args)
        {
            if (CreatingObject != null)
            {
                CreatingObject(Instance, args);
            }
        }

        public static void OnCreatedObject(ObjectEventArgs args)
        {
            if (CreatedObject != null)
            {
                CreatedObject(Instance, args);
            }
        }

        public static void OnDeletingObjects(DeleteEventArgs args)
        {
            if (DeletingObjects != null)
            {
                DeletingObjects(Instance, args);
            }
        }

        public static void OnDeletedObjects(DeleteEventArgs args)
        {
            if (DeletedObjects != null)
            {
                DeletedObjects(Instance, args);
            }
        }
    }
}