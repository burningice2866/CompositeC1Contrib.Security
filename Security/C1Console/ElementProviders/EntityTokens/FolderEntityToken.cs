using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.Security.C1Console.ElementProviders.EntityTokens
{
    [SecurityAncestorProvider(typeof(FolderAncestorProvider))]
    public class FolderEntityToken : EntityToken
    {
        public override string Id { get; }
        public override string Source => String.Empty;
        public override string Type => String.Empty;

        public FolderEntityToken(string name)
        {
            Id = name;
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            DoDeserialize(serializedEntityToken, out string _, out string _, out string id);

            return new FolderEntityToken(id);
        }
    }

    public class FolderAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var token = entityToken as FolderEntityToken;
            if (token == null)
            {
                yield break;
            }

            if (token.Id == "Users")
            {
                yield return new SecurityElementProviderEntityToken();
            }
            else
            {
                yield return new FolderEntityToken("Users");
            }
        }
    }
}
