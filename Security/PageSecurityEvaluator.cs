using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security
{
    public class PageSecurityEvaluator : SecurityEvaluator, ISecurityEvaluatorFor<IPage>
    {
        private static readonly ConcurrentDictionary<Guid, EvaluatedPermissions> Cache = new ConcurrentDictionary<Guid, EvaluatedPermissions>();
        private static IDictionary<Guid, IPagePermissions> _permissionsCache;

        static PageSecurityEvaluator()
        {
            DataEvents<IPagePermissions>.OnAfterAdd += Flush;
            DataEvents<IPagePermissions>.OnAfterUpdate += Flush;
            DataEvents<IPagePermissions>.OnDeleted += Flush;

            BuildPermissionsCache();
        }

        private static void Flush(object sender, DataEventArgs e)
        {
            Cache.Clear();

            BuildPermissionsCache();
        }

        private static void BuildPermissionsCache()
        {
            using (var data = new DataConnection())
            {
                _permissionsCache = data.Get<IPagePermissions>().ToDictionary(p => p.PageId);
            }
        }

        public bool HasAccess(IPage page)
        {
            var e = GetEvaluatedPermissions(page);

            return PermissionsFacade.HasAccess(e);
        }

        public EvaluatedPermissions GetEvaluatedPermissions(IPage page)
        {
            return Cache.GetOrAdd(page.Id, g =>
            {
                IPagePermissions permission;
                _permissionsCache.TryGetValue(page.Id, out permission);

                return EvaluatePermissions(permission, p => EvaluateInheritedPermissions(page.Id, p));
            });
        }

        private static void EvaluateInheritedPermissions(Guid current, EvaluatedPermissions evaluatedPermissions)
        {
            var allowedRoles = new List<string>();
            var deniedRolews = new List<string>();

            while ((current = PageManager.GetParentId(current)) != Guid.Empty)
            {
                IPagePermissions permissions;
                if (!_permissionsCache.TryGetValue(current, out permissions))
                {
                    continue;
                }

                if (permissions.DisableInheritance)
                {
                    break;
                }

                var ar = PermissionsFacade.Split(permissions.AllowedRoles);
                var dr = PermissionsFacade.Split(permissions.DeniedRoles);

                allowedRoles.AddRange(ar);
                deniedRolews.AddRange(dr);
            }

            evaluatedPermissions.InheritedAllowedRules = allowedRoles.ToArray();
            evaluatedPermissions.InheritedDenieddRules = deniedRolews.ToArray();
        }
    }
}
