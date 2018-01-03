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
        private IPage Page => GetDataItemFromEntityToken<IPage>();

        public EditWebsiteSecuritySettingsWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Security\\EditWebsiteSecuritySettingsWorkflow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist(nameof(IWebsiteSecuritySettings.WebsiteId)))
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

                Bindings.Add(nameof(IWebsiteSecuritySettings.WebsiteId), settings.WebsiteId);
                Bindings.Add(nameof(IWebsiteSecuritySettings.LoginPageId), settings.LoginPageId);
                Bindings.Add(nameof(IWebsiteSecuritySettings.ForgotPasswordPageId), settings.ForgotPasswordPageId);
                Bindings.Add(nameof(IWebsiteSecuritySettings.AfterLoginPageId), settings.AfterLoginPageId);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var loginPageId = GetBinding<Guid?>(nameof(IWebsiteSecuritySettings.LoginPageId));
            var forgotPasswordPageId = GetBinding<Guid?>(nameof(IWebsiteSecuritySettings.ForgotPasswordPageId));
            var afterLoginPageId = GetBinding<Guid?>(nameof(IWebsiteSecuritySettings.AfterLoginPageId));

            using (var data = new DataConnection())
            {
                var settings = data.Get<IWebsiteSecuritySettings>().Single(s => s.WebsiteId == Page.Id);

                settings.LoginPageId = loginPageId;
                settings.ForgotPasswordPageId = forgotPasswordPageId;
                settings.AfterLoginPageId = afterLoginPageId;

                data.Update(settings);
            }

            var treeRefresher = CreateParentTreeRefresher();
            treeRefresher.PostRefreshMesseges(EntityToken);

            SetSaveStatus(true);
        }
    }
}
