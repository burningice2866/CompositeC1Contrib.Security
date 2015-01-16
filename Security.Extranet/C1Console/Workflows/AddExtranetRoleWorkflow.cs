using System;
using System.Linq;

using Composite.Data;

using CompositeC1Contrib.Security.Extranet.Data.Types;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Security.Extranet.C1Console.Workflows
{
    public class AddExtranetRoleWorkflow : Basic1StepDialogWorkflow
    {
        public AddExtranetRoleWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Security.Extranet\\AddExtranetRoleWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Name"))
            {
                return;
            }

            Bindings.Add("Name", String.Empty);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var name = GetBinding<string>("Name");

            using (var data = new DataConnection())
            {
                var role = data.CreateNew<IExtranetRole>();

                role.Name = name;

                data.Add(role);
            }

            CreateAddNewTreeRefresher(EntityToken).PostRefreshMesseges(EntityToken);
        }

        public override bool Validate()
        {
            var name = GetBinding<string>("Name");

            using (var data = new DataConnection())
            {
                var roleExists = data.Get<IExtranetRole>().Any(r => r.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (roleExists)
                {
                    ShowFieldMessage("Name", "Role already exists");

                    return false;
                }
            }

            return base.Validate();
        }
    }
}
