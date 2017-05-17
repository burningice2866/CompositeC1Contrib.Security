using System.Configuration;

namespace CompositeC1Contrib.Security.Configuration
{
    public class SecuritySection : ConfigurationSection
    {
        private const string ConfigPath = "compositeC1Contrib/security";

        [ConfigurationProperty("profileResolver", IsRequired = false)]
        public string ProfileResolver
        {
            get => (string)this["profileResolver"];
            set => this["profileResolver"] = value;
        }

        [ConfigurationProperty("editProfileHandler", IsRequired = false)]
        public string EditProfileHandler
        {
            get => (string)this["editProfileHandler"];
            set => this["editProfileHandler"] = value;
        }

        public static SecuritySection GetSection()
        {
            return (SecuritySection)ConfigurationManager.GetSection(ConfigPath);
        }
    }
}
