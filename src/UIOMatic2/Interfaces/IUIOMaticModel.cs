using System;
using System.Collections.Generic;

namespace UIOMatic.Interfaces
{
    public interface IUIOMaticModel
    {
        IEnumerable<Exception> Validate();
    }
}