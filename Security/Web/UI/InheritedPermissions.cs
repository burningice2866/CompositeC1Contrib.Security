using System;
using System.Web.UI.WebControls;

using Composite.C1Console.Forms;
using Composite.Plugins.Forms.WebChannel.UiControlFactories;

namespace CompositeC1Contrib.Security.Web.UI
{
    public class InheritedPermissions : UserControlBasedUiControl
    {
        protected Repeater rptAllowedRoles;
        protected Repeater rptDeniedRoles;
        protected PlaceHolder plcNoInheritance;

        [FormsProperty]
        [BindableProperty]
        public EvaluatedPermissions EvaluatedPermissions { get; set; }

        protected override void OnPreRender(EventArgs e)
        {
            if (EvaluatedPermissions.InheritedAllowedRules.Length > 0)
            {
                rptAllowedRoles.Visible = true;
                rptAllowedRoles.DataSource = EvaluatedPermissions.InheritedAllowedRules;
            }

            if (EvaluatedPermissions.InheritedDenieddRules.Length > 0)
            {
                rptDeniedRoles.Visible = true;
                rptDeniedRoles.DataSource = EvaluatedPermissions.InheritedDenieddRules;
            }

            if (!rptAllowedRoles.Visible && !rptDeniedRoles.Visible)
            {
                plcNoInheritance.Visible = true;
            }

            DataBind();

            base.OnPreRender(e);
        }

        public override void BindStateToControlProperties() { }
        public override void InitializeViewState() { }
    }
}
