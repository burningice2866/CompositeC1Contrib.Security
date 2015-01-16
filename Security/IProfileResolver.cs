using System.Web.Security;

namespace CompositeC1Contrib.Security
{
    public interface IProfileResolver
    {
        object Resolve(MembershipUser user);
    }
}
