using System;
using System.Linq;

using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Security.Data.Types;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Security.C1Console.Workflows
{
    public class EditWebsiteSecuritySettingsWorkflow : Basic1StepDocumentWorkflow
    {
        private IPage Page
        {
            get
            {
                var dataToken = (DataEntityToken)EntityToken;

                return (IPage)dataToken.Data;
            }
        }

        public EditWebsiteSecuritySettingsWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Security\\EditWebsiteSecuritySettingsWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("WebsiteId"))
            {
                return;
            }

            using (var data = new DataConnection())
            {
                var settings = data.Get<IWebsiteSecuritySettings>().SingleOrDefault(s => s.WebsiteId == Page.Id);
                if (settings == null)
                {
                    settings = data.CreateNew<IWebsiteSecuritySettings>();

                    settings.WebsiteId = Page.Id;

                    data.Add(settings);
                }

                Bindings.Add("WebsiteId", settings.WebsiteId);
                Bindings.Add("LoginPageId", settings.LoginPageId);
                Bindings.Add("ForgotPasswordPageId", settings.ForgotPasswordPageId);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var loginPageId = GetBinding<Guid?>("LoginPageId");
            var forgotPasswordPageId = GetBinding<Guid?>("ForgotPasswordPageId");

            using (var data = new DataConnection())
            {
                var settings = data.Get<IWebsiteSecuritySettings>().Single(s => s.WebsiteId == Page.Id);

                settings.LoginPageId = loginPageId;
                settings.ForgotPasswordPageId = forgotPasswordPageId;

                data.Update(settings);
            }

            var treeRefresher = CreateParentTreeRefresher();
            treeRefresher.PostRefreshMesseges(EntityToken);

            SetSaveStatus(true);
        }
    }
}
