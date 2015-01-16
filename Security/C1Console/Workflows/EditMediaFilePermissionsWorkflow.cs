using System;
using System.Linq;

using Composite.C1Console.Workflow;
using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public sealed class EditMediaFilePermissionsWorkflow : BaseEditPermissionsWorkflow<IMediaFilePermissions, IMediaFile>
    {
        protected override EvaluatedPermissions GetEvaluatedPermissions()
        {
            return SecurityEvaluatorFactory.GetEvaluatorFor<IMediaFile>().GetEvaluatedPermissions(DataEntity);
        }

        protected override IMediaFilePermissions GetPermissions()
        {
            using (var data = new DataConnection())
            {
                return data.Get<IMediaFilePermissions>().SingleOrDefault(p => p.KeyPath == DataEntity.KeyPath);
            }
        }

        protected override void SavePermissions(IMediaFilePermissions permissions)
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
