using System;

using Composite.Data;

using CompositeC1Contrib.Security.Data.Types;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Security.C1Console.Workflows
{
    public abstract class BaseEditPermissionsWorkflow<TPermissionType, TEntityType> : Basic1StepDialogWorkflow
        where TPermissionType : class, IDataPermissions
        where TEntityType : IData
    {
        protected BaseEditPermissionsWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Security\\EditPermissionsWorkflow.xml") { }

        protected TEntityType DataEntity
        {
            get
            {
                var dataToken = (DataEntityToken)EntityToken;

                return (TEntityType)dataToken.Data;
            }
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Permissions"))
            {
                return;
            }

            var permissions = GetPermissions();
            if (permissions == null)
            {
                using (var data = new DataConnection())
                {
                    permissions = data.CreateNew<TPermissionType>();
                }
            }

            Bindings.Add("EvaluatedPermissions", GetEvaluatedPermissions());
            Bindings.Add("Permissions", permissions);
            Bindings.Add("Title", permissions.GetTypeTitle());
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var permissions = GetBinding<TPermissionType>("Permissions");

            SavePermissions(permissions);

            var treeRefresher = CreateParentTreeRefresher();
            treeRefresher.PostRefreshMesseges(EntityToken);

            SetSaveStatus(true);
        }

        protected abstract EvaluatedPermissions GetEvaluatedPermissions();
        protected abstract TPermissionType GetPermissions();
        protected abstract void SavePermissions(TPermissionType permissions);
    }
}
