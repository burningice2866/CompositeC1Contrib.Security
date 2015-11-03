using System;
using System.Collections.Generic;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementActionProvider;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Security.C1Console.Workflows;

namespace CompositeC1Contrib.Security.C1Console
{
    [ConfigurationElementType(typeof(NonConfigurableElementActionProvider))]
    public class ElementPermissionsActionProvider : IElementActionProvider
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup("Default", ActionGroupPriority.PrimaryLow);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = false, ActionGroup = ActionGroup };

        public IEnumerable<ElementAction> GetActions(EntityToken entityToken)
        {
            var list = new List<ElementAction>();

            var dataToken = entityToken as DataEntityToken;
            if (dataToken == null)
            {
                return list;
            }

            AddWebsiteActions(dataToken, list);
            AddDataActions(list);

            return list;
        }

        private static void AddWebsiteActions(DataEntityToken dataToken, ICollection<ElementAction> list)
        {
            var pageData = dataToken.Data as IPage;
            if (pageData == null)
            {
                return;
            }

            var parentId = PageManager.GetParentId(pageData.Id);
            if (parentId != Guid.Empty)
            {
                return;
            }

            var actionToken = new WorkflowActionToken(typeof(EditWebsiteSecuritySettingsWorkflow));

            list.Add(new ElementAction(new ActionHandle(actionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Edit security settings",
                    ToolTip = "Edit security settings",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            });
        }

        private static void AddDataActions(ICollection<ElementAction> list)
        {
            var actionToken = new WorkflowActionToken(typeof (EditPermissionsWorkflow));

            list.Add(new ElementAction(new ActionHandle(actionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Edit membership permissions",
                    ToolTip = "Edit membership permissions",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            });
        }
    }
}