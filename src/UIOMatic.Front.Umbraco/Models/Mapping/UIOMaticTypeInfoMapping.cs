using NPoco.FluentMappings;
using UIOMatic.Front.Umbraco.Models;
using Umbraco.Cms.Core.Mapping;

namespace UIOMatic.Front.Umbraco.Models.Mapping
{
    public class UIOMaticTypeInfoMapping : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<UIOMatic.Models.UIOMaticTypeInfo, UIOMaticTypeInfo>((source, context) => new UIOMaticTypeInfo(), Map);
         
        }

        private void Map(UIOMatic.Models.UIOMaticTypeInfo source, UIOMaticTypeInfo target, MapperContext context)
        {
            target.Alias = source.Alias;
            target.DisplayNamePlural = source.DisplayNamePlural;
            target.DisplayNameSingular = source.DisplayNameSingular;
            target.AutoIncrementPrimaryKey = source.AutoIncrementPrimaryKey;
            target.DateCreatedFieldKey = source.DateCreatedFieldKey;
            target.DateModifiedFieldKey = source.DateModifiedFieldKey;
            target.EditableProperties = source.EditableProperties;
            target.FolderIcon  = source.FolderIcon;
            target.ItemIcon =  source.ItemIcon;
            target.ListViewActions = source.ListViewActions;
            target.ListViewProperties = source.ListViewProperties;
            target.ListViewFilterProperties = source.ListViewFilterProperties;
            target.Name = source.Name;
            target.NameFieldKey = source.NameFieldKey;
            target.Path = source.Path;
            target.PrimaryKeyColumnName = source.PrimaryKeyColumnName;
            target.RawProperties = source.RawProperties;
            target.ReadOnly = source.ReadOnly;
            target.RenderType= source.RenderType;
            target.SortColumn = source.SortColumn;
            target.SortOrder = source.SortOrder;
            target.TableName = source.TableName;
            target.Type = source.Type;
        }

    }
}
