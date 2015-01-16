using System;
using System.Collections.Generic;

using Composite.Data;
using Composite.Data.Types;

namespace CompositeC1Contrib.Security
{
    public class SecurityEvaluatorFactory
    {
        private static readonly IDictionary<Type, object> Evaluators = new Dictionary<Type, object>()
        {
            { typeof (IPage), new PageSecurityEvaluator() },
            { typeof (IMediaFile), new MediaSecurityEvaluator() },
            { typeof (IMediaFileFolder), new MediaSecurityEvaluator() }
        };

        public static ISecurityEvaluatorFor<T> GetEvaluatorFor<T>() where T : IData
        {
            var type = typeof(T);

            return (ISecurityEvaluatorFor<T>)GetEvaluatorFor(type);
        }

        public static object GetEvaluatorFor(Type type)
        {
            object evaluator;
            if (!Evaluators.TryGetValue(type, out evaluator))
            {
                throw new InvalidOperationException("No security evaluator found");
            }

            return evaluator;
        }
    }
}
