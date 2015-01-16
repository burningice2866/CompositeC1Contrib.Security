using System;
using System.Linq;
using System.Web.Security;

using Composite.Data;

using CompositeC1Contrib.Security.Extranet.Data.Types;
using CompositeC1Contrib.Security.Web;

namespace CompositeC1Contrib.Security.Extranet.Web
{
    public class ExtranetRoleProvider : CompositeC1RoleProvider
    {
        public override string ApplicationName { get; set; }

        public override string[] GetAllRoles()
        {
            var allRoles = base.GetAllRoles().ToList();

            using (var data = new DataConnection())
            {
                var roles = data.Get<IExtranetRole>().Select(r => r.Name);

                allRoles.AddRange(roles);
            }

            return allRoles.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            var roles = base.GetRolesForUser(username).ToList();
            var membershipUser = Membership.GetUser(username);
            var profile = ProfileFacade.GetProfileForUser<IExtranetUser>(membershipUser);
            var profileRoles = profile.RoleNames.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            roles.AddRange(profileRoles);

            return roles.ToArray();
        }
    }
}