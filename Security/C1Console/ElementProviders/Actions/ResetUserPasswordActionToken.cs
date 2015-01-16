using System;
using System.Collections.Generic;
using System.Web.Security;

using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;

namespace CompositeC1Contrib.Security.C1Console.ElementProviders.Actions
{
    [ActionExecutor(typeof(ResetUserPasswordActionExecutor))]
    public class ResetUserPasswordActionToken : ActionToken
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
            return new ResetUserPasswordActionToken();
        }
    }

    public class ResetUserPasswordActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var user = Membership.GetUser(entityToken.Id);
            var newPassword = user.ResetPassword();

            var messageService = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>();

            messageService.ShowMessage(DialogType.Message, "New password", String.Format("The new password is: '{0}'", newPassword));

            return null;
        }
    }
}
