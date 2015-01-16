using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementProvider;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Core.WebClient;
using Composite.Data;

using CompositeC1Contrib.Security.C1Console.ElementProviders.EntityTokens;
using CompositeC1Contrib.Security.C1Console.Workflows;

namespace CompositeC1Contrib.Security.C1Console.ElementProviders
{
    public class SecurityElementProvider : IHooklessElementProvider
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        private const string UrlTemplate = "InstalledPackages/CompositeC1Contrib.Security/UsersList.aspx?type={0}";

        private ElementProviderContext _context;
        public ElementProviderContext Context
        {
            set { _context = value; }
        }

        public IEnumerable<Element> GetChildren(EntityToken entityToken, SearchToken searchToken)
        {
            var folderToken = entityToken as FolderEntityToken;
            if (folderToken != null)
            {
                var folderName = folderToken.Id;
                switch (folderName)
                {
                    case "Users":
                        {
                            int count;
                            var users = Membership.GetAllUsers(0, int.MaxValue, out count).Cast<MembershipUser>().ToList();

                            var approvedCount = users.Count(u => u.IsApproved);

                            var approvedUsersElementHandle = _context.CreateElementHandle(new FolderEntityToken("Approved"));
                            var approvedUsersElement = new Element(approvedUsersElementHandle)
                            {
                                VisualData = new ElementVisualizedData
                                {
                                    Label = String.Format("Approved ({0})", approvedCount),
                                    ToolTip = String.Format("Approved ({0})", approvedCount),
                                    HasChildren = false,
                                    Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                                    OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                                }
                            };

                            AddViewAction(approvedUsersElement, "Approved");

                            yield return approvedUsersElement;

                            var notApprovedCount = users.Count(u => !u.IsApproved);

                            var notApprovedUsersElementHandle = _context.CreateElementHandle(new FolderEntityToken("NotApproved"));
                            var notApprovedUsersElement = new Element(notApprovedUsersElementHandle)
                            {
                                VisualData = new ElementVisualizedData
                                {
                                    Label = String.Format("Not approved ({0})", notApprovedCount),
                                    ToolTip = String.Format("Not approved ({0})", notApprovedCount),
                                    HasChildren = false,
                                    Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                                    OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                                }
                            };

                            AddViewAction(notApprovedUsersElement, "NotApproved");

                            yield return notApprovedUsersElement;
                        }

                        break;
                }
            }

            if (entityToken is SecurityElementProviderEntityToken)
            {
                var usersElementHandle = _context.CreateElementHandle(new FolderEntityToken("Users"));
                var usersElement = new Element(usersElementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = "Users",
                        ToolTip = "Users",
                        HasChildren = true,
                        Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                        OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                    }
                };

                var createActionToken = new WorkflowActionToken(typeof(AddUserWorkflow), new[] { PermissionType.Add, PermissionType.Administrate });
                usersElement.AddAction(new ElementAction(new ActionHandle(createActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Add",
                        ToolTip = "Add",
                        Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                        ActionLocation = ActionLocation
                    }
                });

                yield return usersElement;
            }
        }

        private static void AddViewAction(Element element, string type)
        {
            var url = String.Format(UrlTemplate, type);
            url = UrlUtils.ResolveAdminUrl(url);

            var viewUrlAction = new UrlActionToken("View users", url, new[] { PermissionType.Read });
            element.AddAction(new ElementAction(new ActionHandle(viewUrlAction))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "View users",
                    ToolTip = "View users",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            });
        }

        public IEnumerable<Element> GetRoots(SearchToken searchToken)
        {
            var elementHandle = _context.CreateElementHandle(new SecurityElementProviderEntityToken());
            var rootElement = new Element(elementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Security",
                    ToolTip = "Security",
                    HasChildren = true,
                    Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                    OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                }
            };

            return new[] { rootElement };
        }
    }
}
