using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;

namespace UIOMatic.Front.Umbraco.ContentApps
{
    public interface IUiomaticContentAppFactory
    {
        ContentApp GetContentAppFor(Type type, IEnumerable<IReadOnlyUserGroup> userGroups);
    }
}
