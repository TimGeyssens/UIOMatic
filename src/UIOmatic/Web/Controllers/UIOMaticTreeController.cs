using System;
using System.Linq;
using System.Reflection;
using umbraco;
using umbraco.BusinessLogic.Actions;
using UIOMatic;
using UIOMatic.Attributes;
using UIOMatic.Controllers;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace UIOmatic.Web.Controllers
{
    [Tree("uiomatic", "uioMaticTree", "UI-O-Matic")]
    [PluginController("UIOMatic")]
    public class UIOMaticTreeController : TreeController
    {
        protected override Umbraco.Web.Models.Trees.TreeNodeCollection GetTreeNodes(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            var types = Helper.GetTypesWithUIOMaticAttribute().OrderBy(x=> x.Name);

            //check if we're rendering the root node's children
            if (id == "-1")
            {
                var nodes = new TreeNodeCollection();
                foreach (var type in types)
                {
                    var attri = (UIOMaticAttribute)Attribute.GetCustomAttribute(type, typeof(UIOMaticAttribute));

                    if (attri.RenderType == UIOMatic.Enums.UIOMaticRenderType.Tree)
                    {
                        var node = this.CreateTreeNode(
                            type.AssemblyQualifiedName,
                            "-1",
                            queryStrings,
                            attri.Name,
                            attri.FolderIcon,
                            true);

                        nodes.Add(node);
                    }
                    else
                    {
                        var node = this.CreateTreeNode(
                            type.AssemblyQualifiedName,
                            "-1",
                            queryStrings,
                            attri.Name,
                            attri.FolderIcon,
                            false,
                            "uiomatic/uioMaticTree/list/" + type.AssemblyQualifiedName);

                        nodes.Add(node);
                    }
                }
                return nodes;

            }
            
            if (types.Any(x => x.AssemblyQualifiedName == id))
            {
                var ctrl = new PetaPocoObjectController();
                var nodes = new TreeNodeCollection();

                var currentType = types.SingleOrDefault(x => x.AssemblyQualifiedName == id);
                var attri = (UIOMaticAttribute)Attribute.GetCustomAttribute(currentType, typeof(UIOMaticAttribute));

                
                var itemIdPropName = string.Empty;
                var primKeyAttri = currentType.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
                if (primKeyAttri.Any())
                    itemIdPropName = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;

                if (string.IsNullOrWhiteSpace(itemIdPropName))
                {
                    foreach (var property in currentType.GetProperties())
                    {
                        var keyAttri = property.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyColumnAttribute));
                        if (!keyAttri.Any()) continue;
                        var columnAttri =
                            property.GetCustomAttributes().Where(x => x.GetType() == typeof(ColumnAttribute));
                        itemIdPropName = property.Name;
                    }
                }
                else
                {
                    itemIdPropName = "Id";
                }


                foreach (dynamic item in ctrl.GetAll(id, attri.SortColumn, attri.SortOrder))
                {


                    var node = CreateTreeNode(
                        item.GetType().GetProperty(itemIdPropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(item, null).ToString() + "?type=" + id,
                        id,
                        queryStrings,
                        item.ToString(),
                        attri.ItemIcon,
                        false);

                    nodes.Add(node);

                }
                return nodes;

            }

            //this tree doesn't suport rendering more than 2 levels
            throw new NotSupportedException();
        }

        protected override Umbraco.Web.Models.Trees.MenuItemCollection GetMenuForNode(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();

            if (id == "-1")
            {
                menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
            }
            else
            {
                if (id.IndexOf("?") > 0)
                {
                    var currentType = Type.GetType(id.Split('?')[1].Replace("type=",""));
                    var attri = (UIOMaticAttribute)Attribute.GetCustomAttribute(currentType, typeof(UIOMaticAttribute));

                    if (!attri.ReadOnly)
                        menu.Items.Add<ActionDelete>(ui.Text("actions", ActionDelete.Instance.Alias));
                }
                else
                {
                    var currentType = Type.GetType(id);
                    var attri = (UIOMaticAttribute)Attribute.GetCustomAttribute(currentType, typeof(UIOMaticAttribute));

                    if(!attri.ReadOnly)
                        menu.Items.Add<CreateChildEntity, ActionNew>(ui.Text("actions", ActionNew.Instance.Alias));

                    if(attri.RenderType == UIOMatic.Enums.UIOMaticRenderType.Tree)
                        menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
                }


            }
            return menu;
        }

    }
}