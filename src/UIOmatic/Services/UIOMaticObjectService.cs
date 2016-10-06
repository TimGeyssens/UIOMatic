using System;
using UIOMatic;
using UIOMatic.Interfaces;

namespace UIOmatic.Services
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

        private static Lazy<IUIOMaticObjectService> _instance = new Lazy<IUIOMaticObjectService>(() => (IUIOMaticObjectService)Activator.CreateInstance(Config.DefaultObjectServiceType, null));

        private UIOMaticObjectService()
        { }

        public static IUIOMaticObjectService Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public static void OnBuildingQuery(IUIOMaticObjectService instance, QueryEventArgs args)
        {
            if (BuildingQuery != null)
            {
                BuildingQuery(instance, args);
            }
        }

        public static void OnBuiltQuery(IUIOMaticObjectService instance, QueryEventArgs args)
        {
            if (BuiltQuery != null)
            {
                BuiltQuery(instance, args);
            }
        }

        public static void OnScaffoldingObject(IUIOMaticObjectService instance, ObjectEventArgs args)
        {
            if (ScaffoldingObject != null)
            {
                ScaffoldingObject(instance, args);
            }
        }

        public static void OnUpdatingObject(IUIOMaticObjectService instance, ObjectEventArgs args)
        {
            if (UpdatingObject != null)
            {
                UpdatingObject(instance, args);
            }
        }

        public static void OnUpdatedObject(IUIOMaticObjectService instance, ObjectEventArgs args)
        {
            if (UpdatedObject != null)
            {
                UpdatedObject(instance, args);
            }
        }

        public static void OnCreatingObject(IUIOMaticObjectService instance, ObjectEventArgs args)
        {
            if (CreatingObject != null)
            {
                CreatingObject(instance, args);
            }
        }

        public static void OnCreatedObject(IUIOMaticObjectService instance, ObjectEventArgs args)
        {
            if (CreatedObject != null)
            {
                CreatedObject(instance, args);
            }
        }
    }
}