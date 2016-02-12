using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIOMatic.Interfaces
{
    delegate void Creating(object sender, ObjectEventArgs e);
    delegate void Created(object sender, ObjectEventArgs e);
    delegate void Updating(object sender, ObjectEventArgs e);
    delegate void Updated(object sender, ObjectEventArgs e);
    public interface IUIOMaticModel
    {
        IEnumerable<Exception> Validate();
        void SetDefaultValue();
        //event EventHandler OnCreating;
        //event EventHandler OnCreated;
        //event EventHandler OnUpdating;
        //event EventHandler OnUpdated;
    }
}