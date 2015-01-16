using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Security;

namespace CompositeC1Contrib.Security.Web
{
    public class CompositeC1RoleProvider : RoleProvider
    {
        public static readonly string AuthenticatedRole = "AUTHENTICATED";
        public static readonly string AnonymousdRole = "ANONYMOUS";

        public override string ApplicationName { get; set; }

        public override void Initialize(string name, NameValueCollection config)
        {
            ApplicationName = config["applicationName"];
            if (String.IsNullOrEmpty(ApplicationName))
            {
                throw new ProviderException("Application name needs to be set");
            }

            base.Initialize(name, config);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotSupportedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotSupportedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotSupportedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return new string[0];
        }

        public override string[] GetAllRoles()
        {
            return new[] { AuthenticatedRole, AnonymousdRole };
        }

        public override string[] GetRolesForUser(string username)
        {
            return new [] { AuthenticatedRole };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return new string[0];
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotSupportedException();
        }

        public override bool RoleExists(string roleName)
        {
            return GetAllRoles().Contains(roleName);
        }
    }
}
