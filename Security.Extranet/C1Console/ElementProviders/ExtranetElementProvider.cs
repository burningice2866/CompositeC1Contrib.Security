using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementProvider;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Data;

using CompositeC1Contrib.Security.C1Console.ElementProviders.EntityTokens;
using CompositeC1Contrib.Security.Extranet.C1Console.ElementProviders.Actions;
using CompositeC1Contrib.Security.Extranet.C1Console.Workflows;
using CompositeC1Contrib.Security.Extranet.Data.Types;

namespace CompositeC1Contrib.Security.Extranet.C1Console.ElementProviders
{
    public class ExtranetElementProvider : IHooklessElementProvider, IAuxiliarySecurityAncestorProvider
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        private ElementProviderContext _context;
        public ElementProviderContext Context
        {
            set { _context = value; }
        }

        public ExtranetElementProvider()
        {
            AuxiliarySecurityAncestorFacade.AddAuxiliaryAncestorProvider<DataEntityToken>(this);
        }

        public IEnumerable<Element> GetChildren(EntityToken entityToken, SearchToken searchToken)
        {
            var list = new List<Element>();

            var folderToken = entityToken as FolderEntityToken;
            if (folderToken != null)
            {
                switch (folderToken.Id)
                {
                    case "Users":

                        using (var data = new DataConnection())
                        {
                            var users = data.Get<IExtranetUser>().OrderBy(u => u.FirstName);
                            foreach (var user in users)
                            {
                                var label = user.FirstName + " " + user.LastName;

                                var elementHandle = _context.CreateElementHandle(user.GetDataEntityToken());
                                var element = new Element(elementHandle)
                                {
                                    VisualData = new ElementVisualizedData
                                    {
                                        Label = label,
                                        ToolTip = label,
                                        HasChildren = false,
                                        Icon = new ResourceHandle("Composite.Icons", "user"),
                                        OpenedIcon = new ResourceHandle("Composite.Icons", "user")
                                    }
                                };

                                var editActionHandle = new ActionHandle(new WorkflowActionToken(typeof(EditExtranetUserWorkflow)));
                                element.AddAction(new ElementAction(editActionHandle)
                                {
                                    VisualData = new ActionVisualizedData
                                    {
                                        Label = "Edit",
                                        ToolTip = "Edit",
                                        ActionLocation = ActionLocation,
                                        Icon = new ResourceHandle("Composite.Icons", "generic-edit")
                                    }
                                });

                                var deleteActionHandle = new ActionHandle(new ConfirmWorkflowActionToken("Are you sure you want to delete " + label, typeof(DeleteUserActionToken)));
                                element.AddAction(new ElementAction(deleteActionHandle)
                                {
                                    VisualData = new ActionVisualizedData
                                    {
                                        Label = "Delete",
                                        ToolTip = "Delete",
                                        ActionLocation = ActionLocation,
                                        Icon = new ResourceHandle("Composite.Icons", "generic-delete")
                                    }
                                });

                                list.Add(element);
                            }
                        }

                        break;

                    case "Roles":

                        using (var data = new DataConnection())
                        {
                            var roles = data.Get<IExtranetRole>().OrderBy(u => u.Name);
                            foreach (var role in roles)
                            {
                                var elementHandle = _context.CreateElementHandle(role.GetDataEntityToken());
                                var element = new Element(elementHandle)
                                {
                                    VisualData = new ElementVisualizedData
                                    {
                                        Label = role.Name,
                                        ToolTip = role.Name,
                                        HasChildren = false,
                                        Icon = new ResourceHandle("Composite.Icons", "user-group"),
                                        OpenedIcon = new ResourceHandle("Composite.Icons", "user-group")
                                    }
                                };

                                var deleteActionHandle = new ActionHandle(new ConfirmWorkflowActionToken("Are you sure you want to delete " + role.Name, typeof(DeleteRoleActionToken)));
                                element.AddAction(new ElementAction(deleteActionHandle)
                                {
                                    VisualData = new ActionVisualizedData
                                    {
                                        Label = "Delete",
                                        ToolTip = "Delete",
                                        ActionLocation = ActionLocation,
                                        Icon = new ResourceHandle("Composite.Icons", "generic-delete")
                                    }
                                });

                                list.Add(element);
                            }
                        }

                        break;
                }
            }

            return list;
        }

        public IEnumerable<Element> GetRoots(SearchToken searchToken)
        {
            var usersElementHandle = _context.CreateElementHandle(new FolderEntityToken("Users"));
            var usersElement = new Element(usersElementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Users",
                    ToolTip = "Users",
                    HasChildren = true,
                    Icon = new ResourceHandle("Composite.Icons", "folder"),
                    OpenedIcon = new ResourceHandle("Composite.Icons", "folder-open")
                }
            };

            var addUserActionHandle = new ActionHandle(new WorkflowActionToken(typeof(AddExtranetUserWorkflow)));
            usersElement.AddAction(new ElementAction(addUserActionHandle)
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add",
                    ToolTip = "Add",
                    ActionLocation = ActionLocation,
                    Icon = new ResourceHandle("Composite.Icons", "generic-add")
                }
            });

            var rolesElementHandle = _context.CreateElementHandle(new FolderEntityToken("Roles"));
            var rolesElement = new Element(rolesElementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Roles",
                    ToolTip = "Roles",
                    HasChildren = true,
                    Icon = new ResourceHandle("Composite.Icons", "folder"),
                    OpenedIcon = new ResourceHandle("Composite.Icons", "folder-open")
                }
            };

            var addRoleActionHandle = new ActionHandle(new WorkflowActionToken(typeof(AddExtranetRoleWorkflow)));
            rolesElement.AddAction(new ElementAction(addRoleActionHandle)
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Add",
                    ToolTip = "Add",
                    ActionLocation = ActionLocation,
                    Icon = new ResourceHandle("Composite.Icons", "generic-add")
                }
            });

            return new[] { usersElement, rolesElement };
        }

        public Dictionary<EntityToken, IEnumerable<EntityToken>> GetParents(IEnumerable<EntityToken> entityTokens)
        {
            var dictionary = new Dictionary<EntityToken, IEnumerable<EntityToken>>();
            foreach (var token in entityTokens)
            {
                var dataToken = token as DataEntityToken;
                if (dataToken == null)
                {
                    continue;
                }

                if (dataToken.InterfaceType == typeof(IExtranetUser))
                {
                    dictionary.Add(token, new[] { new FolderEntityToken("Users") });
                }

                if (dataToken.InterfaceType == typeof(IExtranetRole))
                {
                    dictionary.Add(token, new[] { new FolderEntityToken("Roles") });
                }
            }

            return dictionary;
        }
    }
}
