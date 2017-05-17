using System;
using System.Linq;
using System.Web.Security;

using Composite.Data;

using CompositeC1Contrib.Security.Extranet.Data.Types;

namespace CompositeC1Contrib.Security.Extranet
{
    public class ProfileResolver : IProfileResolver
    {
        public object Resolve(MembershipUser user)
        {
            if (user.ProviderUserKey == null)
            {
                throw new ArgumentNullException(nameof(user), "User key is null");
            }

            using (var data = new DataConnection())
            {
                var id = (Guid)user.ProviderUserKey;

                var profile = data.Get<IExtranetUser>().SingleOrDefault(u => u.MemberShipUserId == id);
                if (profile == null)
                {
                    profile = data.CreateNew<IExtranetUser>();

                    profile.Id = Guid.NewGuid();
                    profile.MemberShipUserId = id;
                    profile.FirstName = String.Empty;
                    profile.LastName = String.Empty;
                    profile.RoleNames = String.Empty;

                    profile = data.Add(profile);
                }

                return profile;
            }
        }
    }
}
