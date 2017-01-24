using Composite.Data;

namespace CompositeC1Contrib.Security
{
    public static class DataExtensions
    {
        public static SecurityEvaluator GetSecurityEvaluator(this IData data)
        {
            return SecurityEvaluatorFactory.GetEvaluatorFor(data);
        }
    }
}
