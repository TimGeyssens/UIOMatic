using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Sections;

namespace UIOMatic.Sections
{
    public class SectionComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Sections().Append<UIOMaticSection>();
        }
    }

    public class UIOMaticSection : ISection
    {
        public string Alias => Constants.SectionAlias;

        public string Name => "UI-O-Matic";
    }

   

}
