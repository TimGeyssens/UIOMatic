
using Umbraco.Core.Composing;
using Umbraco.Core.Models.Sections;
using Umbraco.Web;
using Umbraco.Web.Models.Trees;

namespace UIOMatic.Sections
{
    public class SectionComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Sections().Append<UIOMaticSection>();
        }
    }

    public class UIOMaticSection : ISection
    {
        public string Alias => Constants.SectionAlias;

        public string Name => "UI-O-Matic";
    }

   

}
