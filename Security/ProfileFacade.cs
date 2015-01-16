using System;
using System.Web.Security;

using CompositeC1Contrib.Security.Configuration;

namespace CompositeC1Contrib.Security
{
    public class ProfileFacade
    {
        private static readonly IProfileResolver Resolver;

        static ProfileFacade()
        {
            var section = SecuritySection.GetSection();
            if (section == null || String.IsNullOrEmpty(section.ProfileResolver))
            {
                return;
            }

            var resolver = Type.GetType(section.ProfileResolver);
            if (resolver == null)
            {
                return;
            }

            Resolver = Activator.CreateInstance(resolver) as IProfileResolver;
        }

        public static T GetProfile<T>()
        {
            var user = Membership.GetUser();

            return GetProfileForUser<T>(user);
        }

        public static T GetProfileForUser<T>(MembershipUser user)
        {
            if (Resolver == null)
            {
                throw new InvalidOperationException("No resolver defined");
            }

            return (T)Resolver.Resolve(user);
        }
    }
}
