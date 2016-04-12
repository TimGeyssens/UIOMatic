using System;

namespace UIOMatic.Core
{
    public class ObjectEventArgs : EventArgs
    {
        public ObjectEventArgs(object obj)
        {
            this.Object = obj;
        }
        public Object Object { get; set; }
    }
}