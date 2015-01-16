using System;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI.WebControls;

using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.WebClient.FlowMediators;
using Composite.Core.WebClient.UiControlLib;

using CompositeC1Contrib.Security.C1Console.ElementProviders.EntityTokens;
using CompositeC1Contrib.Security.C1Console.Workflows;

namespace CompositeC1Contrib.Security.Web.UI
{
    public class UsersListPage : BasePage
    {
        protected Repeater rptUsers;
        protected Selector PageSize;

        protected Composite.Core.WebClient.UiControlLib.TextBox txtEmail;
        protected Composite.Core.WebClient.UiControlLib.TextBox txtUsername;

        protected ToolbarButton PrevPage;
        protected ToolbarButton NextPage;
        protected Composite.Core.WebClient.UiControlLib.TextBox PageNumber;

        [WebMethod]
        public static void InvokeEditUserWorkflow(string username, string consoleId)
        {
            const string providerName = "SecurityElementProvider";

            var entityToken = new FolderEntityToken("Users");
            var actionToken = new WorkflowActionToken(typeof(EditUserWorkflow), new[] { PermissionType.Edit, PermissionType.Administrate })
            {
                Payload = username
            };

            TreeServicesFacade.ExecuteElementAction(providerName,
                EntityTokenSerializer.Serialize(entityToken, true),
                String.Empty,
                ActionTokenSerializer.Serialize(actionToken, true),
                consoleId);
        }

        protected string Type
        {
            get { return Request.QueryString["type"]; }
        }

        protected void OnFilter(object sender, EventArgs e)
        {

        }

        protected void OnRefresh(object sender, EventArgs e)
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            SetPageNumber(1);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                SetDefaults();
            }

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindControls();

            base.OnPreRender(e);
        }

        protected void Next(object sender, EventArgs e)
        {
            var pageNumber = int.Parse(PageNumber.Text);

            SetPageNumber(pageNumber + 1);
        }

        protected void Prev(object sender, EventArgs e)
        {
            var pageNumber = int.Parse(PageNumber.Text);
            if (pageNumber > 1)
            {
                SetPageNumber(pageNumber - 1);
            }
        }

        private void SetPageNumber(int pageNumber)
        {
            PageNumber.Text = pageNumber.ToString(CultureInfo.InvariantCulture);
        }

        private void BindControls()
        {
            var pageNumber = int.Parse(PageNumber.Text);
            var pageSize = int.Parse(PageSize.SelectedValue);

            int count;

            var users = Membership.GetAllUsers(0, int.MaxValue, out count).Cast<MembershipUser>();

            switch (Type)
            {
                case "Approved": users = users.Where(u => u.IsApproved); break;
                case "NotApproved": users = users.Where(u => !u.IsApproved); break;
            }

            if (!String.IsNullOrEmpty(txtEmail.Text))
            {
                users = users.Where(u => u.Email.Contains(txtEmail.Text));
            }

            if (!String.IsNullOrEmpty(txtUsername.Text))
            {
                users = users.Where(u => u.UserName.Contains(txtUsername.Text));
            }

            var list = users.ToList();

            count = list.Count();
            list = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            PrevPage.Attributes["client_isdisabled"] = (pageNumber == 1).ToString().ToLower();
            NextPage.Attributes["client_isdisabled"] = (count <= (pageSize * pageNumber)).ToString().ToLower();

            rptUsers.DataSource = list;

            DataBind();
        }
    }
}
