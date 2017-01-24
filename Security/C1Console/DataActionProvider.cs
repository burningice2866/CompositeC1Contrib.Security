using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;


using Composite.C1Console.Elements;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.Security.C1Console.Workflows;

namespace CompositeC1Contrib.Security.C1Console
{
    [Export(typeof(IElementActionProviderFor))]
    public class DataActionProvider : IElementActionProviderFor
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup("Default", ActionGroupPriority.PrimaryLow);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = false, ActionGroup = ActionGroup };

        public IEnumerable<Type> ProviderFor
        {
            get { return new[] { typeof(DataEntityToken) }; }
        }

        public void AddActions(Element element)
        {
            var actions = Provide(element.ElementHandle.EntityToken);

            element.AddAction(actions);
        }

        public IEnumerable<ElementAction> Provide(Composite.C1Console.Security.EntityToken entityToken)
        {
            var dataToken = (DataEntityToken)entityToken;

            var websiteAction = CreateWebsiteAction(dataToken);
            if (websiteAction != null)
            {
                yield return websiteAction;
            }

            var dataAction = CreateDataAction();
            if (dataAction != null)
            {
                yield return dataAction;
            }
        }

        private static ElementAction CreateWebsiteAction(DataEntityToken dataToken)
        {
            var pageData = dataToken.Data as IPage;
            if (pageData == null)
            {
                return null;
            }

            var parentId = PageManager.GetParentId(pageData.Id);
            if (parentId != Guid.Empty)
            {
                return null;
            }

            var actionToken = new WorkflowActionToken(typeof(EditWebsiteSecuritySettingsWorkflow));

            return new ElementAction(new ActionHandle(actionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Edit security settings",
                    ToolTip = "Edit security settings",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            };
        }

        private static ElementAction CreateDataAction()
        {
            var actionToken = new WorkflowActionToken(typeof(EditPermissionsWorkflow));

            return new ElementAction(new ActionHandle(actionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Edit membership permissions",
                    ToolTip = "Edit membership permissions",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            };
        }
    }
}