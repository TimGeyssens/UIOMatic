using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIOMatic.Interfaces
{
    public interface IUIOMaticModel
    {
        string UmbracoTreeNodeName
        {
            get;
            
        }

        IEnumerable<Exception> Validate();
    }
}