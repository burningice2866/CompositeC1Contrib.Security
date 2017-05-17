using System;
using System.Collections.Generic;

using Composite.Data;
using Composite.Data.Types;

namespace CompositeC1Contrib.Security
{
    public class SecurityEvaluatorFactory
    {
        private static readonly IDictionary<Type, SecurityEvaluator> Evaluators = new Dictionary<Type, SecurityEvaluator>
        {
            { typeof (IPage), new PageSecurityEvaluator() },
            { typeof (IMediaFile), new MediaSecurityEvaluator() },
            { typeof (IMediaFileFolder), new MediaSecurityEvaluator() }
        };

        public static SecurityEvaluator GetEvaluatorFor(IData data)
        {
            var type = data.DataSourceId.InterfaceType;

            return GetEvaluatorFor(type);
        }

        public static SecurityEvaluator GetEvaluatorFor(Type type)
        {
            return Evaluators.TryGetValue(type, out SecurityEvaluator evaluator) ? evaluator : new SecurityEvaluator();
        }
    }
}
