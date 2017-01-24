using System.Web.Security;

namespace CompositeC1Contrib.Security
{
    public static class MembershipUserExtensions
    {
        public static T GetProfile<T>(this MembershipUser user)
        {
            return ProfileFacade.GetProfileForUser<T>(user);
        }
    }
}
