using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIOMatic
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