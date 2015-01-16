using System;
using System.Web.Security;

using Composite.C1Console.Workflow;
using Composite.Data;

using CompositeC1Contrib.Security.C1Console.Workflows;
using CompositeC1Contrib.Security.Extranet.Data.Types;

namespace CompositeC1Contrib.Security.Extranet.C1Console.Workflows
{
    public class AddExtranetUserWorkflow : AddUserWorkflow
    {
        public AddExtranetUserWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Security.Extranet\\AddExtranetUserWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("FirstName"))
            {
                return;
            }

            Bindings.Add("FirstName", String.Empty);
            Bindings.Add("LastName", String.Empty);

            base.OnInitialize(sender, e);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var userName = GetBinding<string>("UserName");
            var password = GetBinding<string>("Password");
            var email = GetBinding<string>("Email");

            var isApproved = GetBinding<bool>("IsApproved");

            var user = Membership.CreateUser(userName, password, email);

            if (isApproved)
            {
                user.IsApproved = true;

                Membership.UpdateUser(user);
            }

            using (var data = new DataConnection())
            {
                var firstName = GetBinding<string>("FirstName");
                var lastName = GetBinding<string>("LastName");

                var profile = data.CreateNew<IExtranetUser>();

                profile.Id = Guid.NewGuid();
                profile.MemberShipUserId = (Guid)user.ProviderUserKey;
                profile.FirstName = firstName;
                profile.LastName = lastName;
                profile.RoleNames = String.Empty;

                profile = data.Add(profile);

                var editWorkflowToken = new WorkflowActionToken(typeof(EditExtranetUserWorkflow));

                CreateAddNewTreeRefresher(EntityToken).PostRefreshMesseges(EntityToken);
                ExecuteAction(profile.GetDataEntityToken(), editWorkflowToken);
            }
        }

        public override bool Validate()
        {
            return base.Validate();
        }
    }
}
