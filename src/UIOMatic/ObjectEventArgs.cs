using System;

namespace UIOMatic
{
    public class ObjectEventArgs : EventArgs
    {
        public ObjectEventArgs(object obj)
        {
            Object = obj;
        }

        public object Object { get; set; }
    }
}