using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Security.Data;
using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security
{
    public class PageSecurityEvaluator : SecurityEvaluator
    {
        private readonly ConcurrentDictionary<Guid, EvaluatedPermissions> _cache = new ConcurrentDictionary<Guid, EvaluatedPermissions>();

        public PageSecurityEvaluator()
        {
            DataEvents<IDataPermissions>.OnAfterAdd += Flush;
            DataEvents<IDataPermissions>.OnAfterUpdate += Flush;
            DataEvents<IDataPermissions>.OnDeleted += Flush;
        }

        private void Flush(object sender, DataEventArgs e)
        {
            _cache.Clear();
        }

        public override EvaluatedPermissions GetEvaluatedPermissions(IData data)
        {
            if (data is IPage page)
            {
                return _cache.GetOrAdd(page.Id, g =>
                {
                    PermissionsCache.TryGetValue(new CompoundIDataPermissionsKey(data), out IDataPermissions permissions);

                    return EvaluatePermissions(permissions, p => EvaluateInheritedPermissions(page.Id, p));
                });
            }

            return base.GetEvaluatedPermissions(data);
        }

        private static void EvaluateInheritedPermissions(Guid current, EvaluatedPermissions evaluatedPermissions)
        {
            var allowedRoles = new List<string>();
            var deniedRolews = new List<string>();

            while ((current = PageManager.GetParentId(current)) != Guid.Empty)
            {
                if (!PermissionsCache.TryGetValue(new CompoundIDataPermissionsKey(typeof(IPage).GetImmutableTypeId(), current.ToString()), out IDataPermissions permissions))
                {
                    continue;
                }

                var ar = PermissionsFacade.Split(permissions.AllowedRoles);
                var dr = PermissionsFacade.Split(permissions.DeniedRoles);

                allowedRoles.AddRange(ar);
                deniedRolews.AddRange(dr);

                if (permissions.DisableInheritance)
                {
                    break;
                }
            }

            evaluatedPermissions.InheritedAllowedRules = allowedRoles.Distinct().ToArray();
            evaluatedPermissions.InheritedDenieddRules = deniedRolews.Distinct().ToArray();
        }
    }
}
