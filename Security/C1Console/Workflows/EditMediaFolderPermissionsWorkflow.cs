using System;
using System.Linq;

using Composite.C1Console.Workflow;
using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public sealed class EditMediaFolderPermissionsWorkflow : BaseEditPermissionsWorkflow<IMediaFolderPermissions, IMediaFileFolder>
    {
        protected override EvaluatedPermissions GetEvaluatedPermissions()
        {
            return SecurityEvaluatorFactory.GetEvaluatorFor<IMediaFileFolder>().GetEvaluatedPermissions(DataEntity);
        }

        protected override IMediaFolderPermissions GetPermissions()
        {
            using (var data = new DataConnection())
            {
                return data.Get<IMediaFolderPermissions>().SingleOrDefault(p => p.KeyPath == DataEntity.KeyPath);
            }
        }

        protected override void SavePermissions(IMediaFolderPermissions permissions)
        {
            using (var data = new DataConnection())
            {
                if (String.IsNullOrEmpty(permissions.KeyPath))
                {
                    permissions.KeyPath = DataEntity.KeyPath;

                    data.Add(permissions);
                }
                else
                {
                    data.Update(permissions);
                }
            }
        }
    }
}
