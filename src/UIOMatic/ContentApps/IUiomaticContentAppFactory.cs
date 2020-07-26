using System;
using Umbraco.Core.Models.ContentEditing;

namespace UIOMatic.ContentApps
{
    public interface IUiomaticContentAppFactory
    {
        ContentApp GetContentAppFor(Type type);
    }
}
