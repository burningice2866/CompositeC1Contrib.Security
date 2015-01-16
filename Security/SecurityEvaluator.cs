using System;
using System.Linq;

using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security
{
    public abstract class SecurityEvaluator
    {
        protected EvaluatedPermissions EvaluatePermissions(IDataPermissions permissions, Action<EvaluatedPermissions> evaluateMethod)
        {
            var allowedRoles = permissions == null ? null : permissions.AllowedRoles;
            var deniedRoles = permissions == null ? null : permissions.DeniedRoles;

            var evaluatedPermissions = new EvaluatedPermissions
            {
                ExplicitAllowedRoles = PermissionsFacade.Split(allowedRoles).ToArray(),
                ExplicitDeniedRoled = PermissionsFacade.Split(deniedRoles).ToArray()
            };

            if (permissions == null || !permissions.DisableInheritance)
            {
                evaluateMethod(evaluatedPermissions);
            }

            return evaluatedPermissions;
        }
    }
}
