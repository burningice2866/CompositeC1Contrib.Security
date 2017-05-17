using System;
using System.Web.Security;

using Composite.Data;

using CompositeC1Contrib.Security.C1Console.Workflows;
using CompositeC1Contrib.Security.Extranet.Data.Types;

namespace CompositeC1Contrib.Security.Extranet.C1Console.Workflows
{
    public class EditExtranetUserWorkflow : EditUserWorkflow
    {
        public EditExtranetUserWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Security.Extranet\\EditExtranetUserWorkflow.xml") { }

        protected override MembershipUser GetMembershipUser()
        {
            var profile = (IExtranetUser)((DataEntityToken)EntityToken).Data;

            return Membership.GetUser(profile.MemberShipUserId);
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("FirstName"))
            {
                return;
            }

            var profile = (IExtranetUser)((DataEntityToken)EntityToken).Data;

            Bindings.Add("FirstName", profile.FirstName);
            Bindings.Add("LastName", profile.LastName);
            Bindings.Add("RoleNames", profile.RoleNames);

            base.OnInitialize(sender, e);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var user = GetMembershipUser();

            var email = GetBinding<string>("Email");
            var isApproved = GetBinding<bool>("IsApproved");
            var isLockedOut = GetBinding<bool>("IsLockedOut");

            var isDirty = false;

            if (email != user.Email)
            {
                user.Email = email;

                isDirty = true;
            }

            if (isApproved != user.IsApproved)
            {
                user.IsApproved = isApproved;

                isDirty = true;
            }

            if (isDirty)
            {
                Membership.UpdateUser(user);
            }

            if (!isLockedOut && user.IsLockedOut)
            {
                user.UnlockUser();
            }

            var profile = (IExtranetUser)((DataEntityToken)EntityToken).Data;

            var firstName = GetBinding<string>("FirstName");
            var lastName = GetBinding<string>("LastName");
            var roleNames = GetBinding<string>("RoleNames");

            using (var data = new DataConnection())
            {
                profile.FirstName = firstName;
                profile.LastName = lastName;
                profile.RoleNames = roleNames;

                data.Update(profile);
            }

            CreateSpecificTreeRefresher().PostRefreshMesseges(EntityToken);
            SetSaveStatus(true);
        }
    }
}
