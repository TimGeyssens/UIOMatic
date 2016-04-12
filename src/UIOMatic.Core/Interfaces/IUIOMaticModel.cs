using System;
using System.Collections.Generic;

namespace UIOMatic.Core.Interfaces
{
    public interface IUIOMaticModel
    {
        IEnumerable<Exception> Validate();
    }
}