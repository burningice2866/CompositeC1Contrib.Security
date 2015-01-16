using System.Web.Security;

namespace CompositeC1Contrib.Security
{
    public interface IEditProfileHandler
    {
        void Load(MembershipUser user);
        void Save(MembershipUser user);
    }
}
