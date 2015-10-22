using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;
using umbraco;
using umbraco.BusinessLogic.Actions;
using UIOMatic.Attributes;
using UIOMatic.Controllers;
using UIOMatic.Interfaces;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Core.Persistence;

namespace UIOMatic.Trees
{
    [Tree("uiomatic", "uioMaticTree", "UI-O-Matic")]
    [PluginController("UIOMatic")]
    public class UIOMaticTreeController : TreeController
    {
        protected override Umbraco.Web.Models.Trees.TreeNodeCollection GetTreeNodes(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            var types = Helper.GetTypesWithUIOMaticAttribute();

            //check if we're rendering the root node's children
            if (id == "-1")
            {
                var nodes = new TreeNodeCollection();
                foreach (var type in types)
                {
                    var attri = (UIOMaticAttribute)Attribute.GetCustomAttribute(type, typeof(UIOMaticAttribute));

                    if (attri.RenderType == Enums.UIOMaticRenderType.Tree)
                    {
                        var node = CreateTreeNode(
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
                        var node = CreateTreeNode(
                            type.AssemblyQualifiedName,
                            "-1",
                            queryStrings,
                            attri.Name,
                            attri.FolderIcon,
                            false,
                            "uiomatic/uioMaticTree/list/"+type.AssemblyQualifiedName);

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

                var itemIdPropName = "Id";
                foreach (var property in currentType.GetProperties())
                {
                    var keyAttri = property.GetCustomAttributes().Where(x => x.GetType() == typeof(PrimaryKeyColumnAttribute));
                    if (keyAttri.Any())
                    {
                        var columnAttri =
                            property.GetCustomAttributes().Where(x => x.GetType() == typeof (ColumnAttribute));
                        itemIdPropName =property.Name;
                    }
                }

                foreach (dynamic item in ctrl.GetAll(id, attri.SortColumn,attri.SortOrder))
                {

                   
                    var node = CreateTreeNode(
                        item.GetType().GetProperty(itemIdPropName).GetValue(item, null).ToString() + "?type=" + id,
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
                // root actions              
               
                menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
                return menu;
            }
            else
            {
                var idInt = 0;
                if (int.TryParse(id.Split('?')[0], out idInt))
                    menu.Items.Add<ActionDelete>(ui.Text("actions", ActionDelete.Instance.Alias));
                else
                {
                    menu.Items.Add<CreateChildEntity, ActionNew>(ui.Text("actions", ActionNew.Instance.Alias));
                    menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias),true);
                }
                    

            }
            return menu;
        }

    }
}