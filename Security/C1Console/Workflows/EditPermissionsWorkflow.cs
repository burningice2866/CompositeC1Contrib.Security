using System;
using System.Linq;

using Composite.Data;

using CompositeC1Contrib.Security.Data.Types;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Security.C1Console.Workflows
{
    public class EditPermissionsWorkflow : Basic1StepDialogWorkflow
    {
        public EditPermissionsWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Security\\EditPermissionsWorkflow.xml") { }

        protected IData DataEntity => ((DataEntityToken)EntityToken).Data;

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Permissions"))
            {
                return;
            }

            using (var data = new DataConnection())
            {
                var datasourceId = DataEntity.GetImmutableTypeId();
                var dataId = DataEntity.GetUniqueKey().ToString();

                var permissions = data.Get<IDataPermissions>().SingleOrDefault(p => p.DataTypeId == datasourceId && p.DataId == dataId);
                if (permissions == null)
                {
                    permissions = data.CreateNew<IDataPermissions>();
                }

                var evaluaedPermissions = DataEntity.GetSecurityEvaluator().GetEvaluatedPermissions(DataEntity);

                Bindings.Add("EvaluatedPermissions", evaluaedPermissions);
                Bindings.Add("Permissions", permissions);
                Bindings.Add("Title", permissions.GetTypeTitle());
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var permissions = GetBinding<IDataPermissions>("Permissions");

            using (var data = new DataConnection())
            {
                if (permissions.DataTypeId == Guid.Empty)
                {
                    permissions.DataTypeId = DataEntity.GetImmutableTypeId();
                    permissions.DataId = DataEntity.GetUniqueKey().ToString();

                    data.Add(permissions);
                }
                else
                {
                    data.Update(permissions);
                }
            }

            var treeRefresher = CreateParentTreeRefresher();
            treeRefresher.PostRefreshMesseges(EntityToken);

            SetSaveStatus(true);
        }
    }
}
