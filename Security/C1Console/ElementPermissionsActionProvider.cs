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
            var dataToken = entityToken as DataEntityToken;
            if (dataToken == null)
            {
                yield break;
            }

            WorkflowActionToken actionToken = null;

            if (dataToken.Data is IPage)
            {
                actionToken = new WorkflowActionToken(typeof(EditPagePermissionsWorkflow));
            }

            if (dataToken.Data is IMediaFile)
            {
                actionToken = new WorkflowActionToken(typeof(EditMediaFilePermissionsWorkflow));
            }

            if (dataToken.Data is IMediaFileFolder)
            {
                actionToken = new WorkflowActionToken(typeof(EditMediaFolderPermissionsWorkflow));
            }

            if (actionToken != null)
            {
                yield return new ElementAction(new ActionHandle(actionToken))
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
}