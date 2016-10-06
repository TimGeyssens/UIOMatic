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
            var nodes = new TreeNodeCollection();
            var types = Helper.GetTypesWithUIOMaticFolderAttribute().OrderBy(x=> x.Name);
            
            foreach (var type in types)
            {
                var attri = (UIOMaticFolderAttribute)Attribute.GetCustomAttribute(type, typeof(UIOMaticFolderAttribute));
                if (attri.ParentAlias == id)
                {
                    var attri2 = attri as UIOMaticAttribute;
                    if (attri2 != null)
                    {
                        if (id == type.AssemblyQualifiedName)
                        {
                            // List nodes
                            var ctrl = new PetaPocoObjectController();

                            var itemIdPropName = string.Empty;
                            var primKeyAttri = type.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyAttribute));
                            if (primKeyAttri.Any())
                                itemIdPropName = ((PrimaryKeyAttribute)primKeyAttri.First()).Value;

                            if (string.IsNullOrWhiteSpace(itemIdPropName))
                            {
                                foreach (var property in type.GetProperties())
                                {
                                    var keyAttri = property.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyColumnAttribute));
                                    if (!keyAttri.Any())
                                        continue;

                                    itemIdPropName = property.Name;
                                }
                            }
                            else
                            {
                                itemIdPropName = "Id";
                            }

                            foreach (dynamic item in ctrl.GetAll(id, attri2.SortColumn, attri2.SortOrder))
                            {
                                var node = CreateTreeNode(
                                    item.GetType().GetProperty(itemIdPropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(item, null).ToString() + "?type=" + id,
                                    id,
                                    queryStrings,
                                    item.ToString(),
                                    attri2.ItemIcon,
                                    false);

                                nodes.Add(node);

                            }
                        }
                        else
                        {
                            // UIOmatic node
                            if (attri2.RenderType == UIOMatic.Enums.UIOMaticRenderType.Tree)
                            {
                                // Tree node
                                var node = this.CreateTreeNode(
                                    type.AssemblyQualifiedName,
                                    id,
                                    queryStrings,
                                    attri.Name,
                                    attri.FolderIcon,
                                    true);

                                nodes.Add(node);
                            }
                            else
                            {
                                // List view node
                                var node = this.CreateTreeNode(
                                    type.AssemblyQualifiedName,
                                    id,
                                    queryStrings,
                                    attri.Name,
                                    attri.FolderIcon,
                                    false,
                                    "uiomatic/uioMaticTree/list/" + type.AssemblyQualifiedName);

                                nodes.Add(node);
                            }
                        }
                    }
                    else
                    {
                        // Just a folder
                        var node = this.CreateTreeNode(
                               attri.Alias,
                               id,
                               queryStrings,
                               attri.Name,
                               attri.FolderIcon,
                               true,
                               "uiomatic");

                        nodes.Add(node);
                    }
                }
            }

            return nodes;
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