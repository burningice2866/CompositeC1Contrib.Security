using System;
using System.Linq;
using System.Web.Security;
using System.Xml.Linq;

using Composite.C1Console.Actions;
using Composite.C1Console.Forms;
using Composite.C1Console.Forms.DataServices;
using Composite.Core.ResourceSystem;
using Composite.Core.Xml;

using CompositeC1Contrib.Email;
using CompositeC1Contrib.Security.Configuration;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Security.C1Console.Workflows
{
    public class EditUserWorkflow : Basic1StepDocumentWorkflow
    {
        private readonly bool _formDefinitionFileSet;

        public EditUserWorkflow(string formDefinitionFile)
            : base(formDefinitionFile)
        {
            _formDefinitionFileSet = true;
        }

        protected virtual MembershipUser GetMembershipUser()
        {
            return Membership.GetUser(Payload);
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("UserName"))
            {
                return;
            }

            var user = GetMembershipUser();

            Bindings.Add("UserName", user.UserName);
            Bindings.Add("Email", user.Email);

            Bindings.Add("IsApproved", user.IsApproved);
            Bindings.Add("IsLockedOut", user.IsLockedOut);

            Bindings.Add("CreationDate", user.CreationDate.ToLocalTime().ToLongDateString());
            Bindings.Add("LastActivityDate", user.LastActivityDate);
            Bindings.Add("LastLockoutDate", user.LastLockoutDate);
            Bindings.Add("LastLoginDate", user.LastLoginDate);
            Bindings.Add("LastPasswordChangedDate", user.LastPasswordChangedDate);

            if (!_formDefinitionFileSet)
            {
                SetupFormData(user);
            }
        }

        private void SetupFormData(MembershipUser user)
        {
            var markupProvider = new FormDefinitionFileMarkupProvider("\\InstalledPackages\\CompositeC1Contrib.Security\\EditUserWorkflow.xml");

            var formDocument = XDocument.Load(markupProvider.GetReader());
            if (formDocument.Root == null)
            {
                return;
            }

            var layoutXElement = formDocument.Root.Element(Namespaces.BindingForms10 + FormKeyTagNames.Layout);
            if (layoutXElement == null)
            {
                return;
            }

            var tabPanelElements = layoutXElement.Element(Namespaces.BindingFormsStdUiControls10 + "TabPanels");
            if (tabPanelElements == null)
            {
                return;
            }

            var bindingsXElement = formDocument.Root.Element(Namespaces.BindingForms10 + FormKeyTagNames.Bindings);
            var lastTabElement = tabPanelElements.Elements().Last();

            LoadExtraSettings(user, bindingsXElement, lastTabElement);

            DeliverFormData("EditFormField", StandardUiContainerTypes.Document, formDocument.ToString(), Bindings, BindingsValidationRules);
        }

        private void LoadExtraSettings(MembershipUser user, XElement bindingsXElement, XElement lastTabElement)
        {
            var handler = GetEditProfilerHandler();
            if (handler == null)
            {
                return;
            }

            var settingsMarkupProvider = new FormDefinitionFileMarkupProvider("\\InstalledPackages\\CompositeC1Contrib.Security\\EditProfileHandler.xml");
            var formDefinitionElement = XElement.Load(settingsMarkupProvider.GetReader());

            var layout = formDefinitionElement.Element(Namespaces.BindingForms10 + FormKeyTagNames.Layout);
            if (layout == null)
            {
                return;
            }

            var bindings = formDefinitionElement.Element(Namespaces.BindingForms10 + FormKeyTagNames.Bindings);
            if (bindings == null)
            {
                return;
            }

            var settingsTab = new XElement(Namespaces.BindingFormsStdUiControls10 + "PlaceHolder");

            settingsTab.Add(new XAttribute("Label", StringResourceSystemFacade.ParseString("Profile")));
            settingsTab.Add(layout.Elements());

            bindingsXElement.Add(bindings.Elements());

            lastTabElement.AddAfterSelf(settingsTab);

            handler.Load(user);

            foreach (var prop in handler.GetType().GetProperties())
            {
                var value = prop.GetValue(handler, null);

                Bindings.Add(prop.Name, value);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var user = GetMembershipUser();

            var email = GetBinding<string>("Email");
            var isApproved = GetBinding<bool>("IsApproved");
            var isLockedOut = GetBinding<bool>("IsLockedOut");

            bool isDirty = false;

            if (email != user.Email)
            {
                user.Email = email;

                isDirty = true;
            }

            if (isApproved != user.IsApproved)
            {
                user.IsApproved = isApproved;

                isDirty = true;
            }

            if (isDirty)
            {
                Membership.UpdateUser(user);
            }

            if (!isLockedOut && user.IsLockedOut)
            {
                user.UnlockUser();
            }

            SaveExtraSettings(user);

            CreateSpecificTreeRefresher().PostRefreshMesseges(EntityToken);
            SetSaveStatus(true);
        }

        private void SaveExtraSettings(MembershipUser user)
        {
            var handler = GetEditProfilerHandler();
            if (handler == null)
            {
                return;
            }

            foreach (var prop in handler.GetType().GetProperties())
            {
                if (!BindingExist(prop.Name))
                {
                    continue;
                }

                var value = GetBinding<object>(prop.Name);

                prop.SetValue(handler, value, null);
            }

            handler.Save(user);
        }

        public override bool Validate()
        {
            var user = GetMembershipUser();
            var email = GetBinding<string>("Email");

            if (String.IsNullOrEmpty(email))
            {
                ShowFieldMessage("Email", "Email required");

                return false;
            }

            if (MailsFacade.ValidateMailAddress(email))
            {
                ShowFieldMessage("Email", "Provided email is not valid");

                return false;
            }

            if (email != user.Email)
            {
                if (Membership.FindUsersByEmail(email).Count != 0)
                {
                    ShowFieldMessage("email", "Email already exists");

                    return false;
                }
            }

            return base.Validate();
        }

        private static IEditProfileHandler GetEditProfilerHandler()
        {
            var section = SecuritySection.GetSection();
            if (section == null || String.IsNullOrEmpty(section.EditProfileHandler))
            {
                return null;
            }

            var handlerType = Type.GetType(section.EditProfileHandler);
            if (handlerType == null)
            {
                return null;
            }

            var handler = Activator.CreateInstance(handlerType) as IEditProfileHandler;
            if (handler == null)
            {
                return null;
            }

            return handler;
        }
    }
}
