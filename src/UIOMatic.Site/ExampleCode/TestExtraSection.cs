using UIOMatic.Interfaces;
using UIOMatic.Web.Controllers;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Sections;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;

namespace UIOMatic.Site.ExampleCode
{
    public class TestExtraSectionComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddSection<TestExtraSection>();
        }
    }

    public class TestExtraSection : ISection
    {
        public string Alias => "comments";
        public string Name => "Comments";
    }

    [Tree("comments", "uiomatic", TreeTitle = "Comments", SortOrder = 1)]
    [PluginController("UIOMatic")]
    public class TestExtraSectionTreeController : UIOMaticTreeController
    {
        public TestExtraSectionTreeController(ILocalizedTextService localizedTextService, UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection, IMenuItemCollectionFactory menuItemCollectionFactory, IEventAggregator eventAggregator, IUIOMaticHelper helper, IUIOMaticObjectService uioMaticObjectService) : base(localizedTextService, umbracoApiControllerTypeCollection, menuItemCollectionFactory, eventAggregator, helper, uioMaticObjectService)
        {
        }
    }
}
