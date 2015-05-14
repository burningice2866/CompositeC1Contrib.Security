using System;
using System.Collections.Generic;
using System.Linq;

using Composite.Data;

using CompositeC1Contrib.Security.Data;
using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security
{
    public class SecurityEvaluator
    {
        protected static IDictionary<CompoundIDataPermissionsKey, IDataPermissions> PermissionsCache = new Dictionary<CompoundIDataPermissionsKey, IDataPermissions>();

        static SecurityEvaluator()
        {
            DataEvents<IDataPermissions>.OnAfterAdd += Flush;
            DataEvents<IDataPermissions>.OnAfterUpdate += Flush;
            DataEvents<IDataPermissions>.OnDeleted += Flush;

            BuildPermissionsCache();
        }

        private static void Flush(object sender, DataEventArgs e)
        {
            BuildPermissionsCache();
        }

        private static void BuildPermissionsCache()
        {
            using (var data = new DataConnection())
            {
                PermissionsCache = data.Get<IDataPermissions>().ToDictionary(p => new CompoundIDataPermissionsKey
                {
                    DataTypeId = p.DataTypeId,
                    DataId = p.DataId
                });
            }
        }

        public bool HasAccess(IData data)
        {
            var e = GetEvaluatedPermissions(data);

            return PermissionsFacade.HasAccess(e);
        }

        public virtual EvaluatedPermissions GetEvaluatedPermissions(IData data)
        {
            IDataPermissions permissions;
            PermissionsCache.TryGetValue(new CompoundIDataPermissionsKey(data), out permissions);

            return EvaluatePermissions(permissions, null);
        }

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
                if (evaluateMethod != null)
                {
                    evaluateMethod(evaluatedPermissions);
                }
            }

            return evaluatedPermissions;
        }
    }
}
