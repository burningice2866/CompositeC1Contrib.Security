using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

using CompositeC1Contrib.Security.Web;

namespace CompositeC1Contrib.Security.Extranet.C1Console
{
    public static class ConsoleHelpers
    {
        public static IList<string> GetRoleNames()
        {
            var roles = Roles.GetAllRoles().ToList();

            roles.Remove(CompositeC1RoleProvider.AnonymousdRole);
            roles.Remove(CompositeC1RoleProvider.AuthenticatedRole);

            return roles;
        }
    }
}
