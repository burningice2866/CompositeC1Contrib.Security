using System;

using Composite.C1Console.Security;
using Composite.C1Console.Security.SecurityAncestorProviders;

namespace CompositeC1Contrib.Security.C1Console.ElementProviders.EntityTokens
{
    [SecurityAncestorProvider(typeof(NoAncestorSecurityAncestorProvider))]
    public class SecurityElementProviderEntityToken : EntityToken
    {
        public override string Id
        {
            get { return "SecurityElementProviderEntityToken"; }
        }

        public override string Source
        {
            get { return String.Empty; }
        }

        public override string Type
        {
            get { return String.Empty; }
        }

        public static EntityToken Deserialize(string serializedData)
        {
            return new SecurityElementProviderEntityToken();
        }

        public override string Serialize()
        {
            return String.Empty;
        }
    }
}
