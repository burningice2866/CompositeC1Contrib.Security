using System;
using System.Collections.Generic;
using System.Web.Security;

using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;

using CompositeC1Contrib.Security.C1Console.ElementProviders.EntityTokens;

namespace CompositeC1Contrib.Security.C1Console.ElementProviders.Actions
{
    [ActionExecutor(typeof(ApproveUserActionExecutor))]
    public class ApproveUserActionToken : ActionToken
    {
        private static readonly IEnumerable<PermissionType> _permissionTypes = new[] { PermissionType.Edit, PermissionType.Administrate };

        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return _permissionTypes; }
        }

        public override string Serialize()
        {
            return String.Empty;
        }

        public static ActionToken Deserialize(string serializedData)
        {
            return new DeleteUserActionToken();
        }
    }

    public class ApproveUserActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var user = Membership.GetUser(entityToken.Id);

            user.IsApproved = true;

            Membership.UpdateUser(user);

            var folderToken = new FolderEntityToken("Users");
            var messageService = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>();

            new SpecificTreeRefresher(flowControllerServicesContainer).PostRefreshMesseges(folderToken);

            messageService.SelectElement(EntityTokenSerializer.Serialize(entityToken));

            return null;
        }
    }
}
