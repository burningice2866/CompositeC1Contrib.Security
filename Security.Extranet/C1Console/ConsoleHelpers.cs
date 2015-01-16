using System.Collections.Generic;
using System.Linq;

using Composite.Data;

using CompositeC1Contrib.Security.Extranet.Data.Types;

namespace CompositeC1Contrib.Security.Extranet.C1Console
{
    public static class ConsoleHelpers
    {
        public static IList<string> GetRoleNames()
        {
            using (var data = new DataConnection())
            {
                return data.Get<IExtranetRole>().Select(r => r.Name).ToList();
            }
        }
    }
}
