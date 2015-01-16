using System;
using System.Collections.Generic;
using System.Web.Security;

using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.Data;

using CompositeC1Contrib.Security.Extranet.Data.Types;

namespace CompositeC1Contrib.Security.Extranet.C1Console.ElementProviders.Actions
{
    [ActionExecutor(typeof(DeleteUserActionExecutor))]
    public class DeleteUserActionToken : ActionToken
    {
        private static readonly IEnumerable<PermissionType> _permissionTypes = new[] { PermissionType.Delete, PermissionType.Administrate };

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

    public class DeleteUserActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            using (var data = new DataConnection())
            {
                var profile = (IExtranetUser) ((DataEntityToken) entityToken).Data;
                var memberShipUser = Membership.GetUser(profile.MemberShipUserId);

                data.Delete(profile);

                Membership.DeleteUser(memberShipUser.UserName, true);
            }

            new ParentTreeRefresher(flowControllerServicesContainer).PostRefreshMesseges(entityToken);

            return null;
        }
    }
}
