using System.Web.Security;

using Composite.Data;

using CompositeC1Contrib.Security.Extranet.Data.Types;

namespace CompositeC1Contrib.Security.Extranet
{
    public class EditProfileHandler : IEditProfileHandler
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleNames { get; set; }

        public void Load(MembershipUser user)
        {
            var profile = ProfileFacade.GetProfileForUser<IExtranetUser>(user);

            FirstName = profile.FirstName;
            LastName = profile.LastName;
            RoleNames = profile.LastName;
        }

        public void Save(MembershipUser user)
        {
            var profile = ProfileFacade.GetProfileForUser<IExtranetUser>(user);

            using (var data = new DataConnection())
            {
                profile.FirstName = FirstName;
                profile.LastName = LastName;
                profile.RoleNames = RoleNames;
                
                data.Update(profile);
            }
        }
    }
}
