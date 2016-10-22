using System;

namespace UIOMatic
{
    public class ObjectEventArgs : EventArgs
    {
        public ObjectEventArgs(Type objectType, object obj)
        {
            ObjectType = objectType;
            Object = obj;
        }

        public Type ObjectType { get; set; }

        public object Object { get; set; }
    }
}