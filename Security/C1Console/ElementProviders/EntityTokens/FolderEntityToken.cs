using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.Security.C1Console.ElementProviders.EntityTokens
{
    [SecurityAncestorProvider(typeof(FolderAncestorProvider))]
    public class FolderEntityToken : EntityToken
    {
        private readonly string _id;
        public override string Id
        {
            get { return _id; }
        }

        public override string Source
        {
            get { return String.Empty; }
        }

        public override string Type
        {
            get { return String.Empty; }
        }

        public FolderEntityToken(string name)
        {
            _id = name;
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            string type;
            string source;
            string id;

            DoDeserialize(serializedEntityToken, out type, out source, out id);

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
