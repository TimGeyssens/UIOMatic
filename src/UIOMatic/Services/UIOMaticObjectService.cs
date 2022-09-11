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

        public void OnBuildingQuery(QueryEventArgs args)
        {
            BuildingQuery?.Invoke(this, args);
        }

        public void OnBuiltQuery(QueryEventArgs args)
        {
            BuiltQuery?.Invoke(this, args);
        }

        public void OnScaffoldingObject(ObjectEventArgs args)
        {
            ScaffoldingObject?.Invoke(this, args);
        }

        public void OnUpdatingObject(ObjectEventArgs args)
        {
            UpdatingObject?.Invoke(this, args);
        }

        public void OnUpdatedObject(ObjectEventArgs args)
        {
            UpdatedObject?.Invoke(this, args);
        }

        public void OnCreatingObject(ObjectEventArgs args)
        {
            CreatingObject?.Invoke(this, args);
        }

        public void OnCreatedObject(ObjectEventArgs args)
        {
            CreatedObject?.Invoke(this, args);
        }

        public void OnDeletingObjects(DeleteEventArgs args)
        {
            DeletingObjects?.Invoke(this, args);
        }

        public void OnDeletedObjects(DeleteEventArgs args)
        {
            DeletedObjects?.Invoke(this, args);
        }
    }
}