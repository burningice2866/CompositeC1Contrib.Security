using System;
using System.Web.Security;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.Security.C1Console.ElementProviders.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Security.C1Console.Workflows
{
    public class AddUserWorkflow : Basic1StepDialogWorkflow
    {
        public AddUserWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Security\\AddUserWorkflow.xml") { }
        public AddUserWorkflow(string formDefinitionFile) : base(formDefinitionFile) { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("UserName"))
            {
                return;
            }

            Bindings.Add("UserName", String.Empty);
            Bindings.Add("Password", String.Empty);
            Bindings.Add("Email", String.Empty);

            Bindings.Add("IsApproved", true);
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

            var containerEntityToken = new FolderEntityToken("Users");
            var workflowToken = new WorkflowActionToken(typeof(EditUserWorkflow))
            {
                Payload = userName
            };

            CreateAddNewTreeRefresher(EntityToken).PostRefreshMesseges(EntityToken);
            ExecuteAction(containerEntityToken, workflowToken);
        }

        public override bool Validate()
        {
            var userName = GetBinding<string>("UserName");
            var email = GetBinding<string>("Email");

            if (String.IsNullOrEmpty(userName))
            {
                ShowFieldMessage("UserName", "Name required");

                return false;
            }

            if (Membership.FindUsersByName(userName).Count != 0)
            {
                ShowFieldMessage("UserName", "Username already exists");

                return false;
            }

            if (String.IsNullOrEmpty(email))
            {
                ShowFieldMessage("Email", "Email required");

                return false;
            }

            if (!MailAddressValidator.IsValid(email))
            {
                ShowFieldMessage("Email", "Provided email is not valid");

                return false;
            }

            if (Membership.FindUsersByEmail(email).Count != 0)
            {
                ShowFieldMessage("email", "Email already exists");

                return false;
            }

            return base.Validate();
        }
    }
}
