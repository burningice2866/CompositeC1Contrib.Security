using System;
using System.Linq;

namespace CompositeC1Contrib.Security
{
    [Serializable]
    public class EvaluatedPermissions
    {
        public string[] ExplicitAllowedRoles { get; set; }
        public string[] ExplicitDeniedRoled { get; set; }

        public string[] InheritedAllowedRules { get; set; }
        public string[] InheritedDenieddRules { get; set; }

        public string[] AllowedRoles
        {
            get { return ExplicitAllowedRoles.Concat(InheritedAllowedRules).ToArray(); }
        }

        public string[] DeniedRoled
        {
            get { return ExplicitDeniedRoled.Concat(InheritedDenieddRules).ToArray(); }
        }

        public EvaluatedPermissions()
        {
            ExplicitAllowedRoles = new string[0];
            ExplicitDeniedRoled = new string[0];

            InheritedAllowedRules = new string[0];
            InheritedDenieddRules = new string[0];
        }
    }
}