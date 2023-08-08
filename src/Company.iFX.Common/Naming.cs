using System.Diagnostics;

namespace Company.iFX.Common
{
    public static class Naming
    {
        public static string[] NamespaceSegments<I>()
        {
            Debug.Assert(typeof(I).IsInterface);
            string[] namespaceSegments = typeof(I)?.Namespace?.Split('.') ?? Array.Empty<string>();

            if (namespaceSegments.Length != Constant.NamespaceSize)
            {
                throw new FormatException($@"Namespace is an invalid format for type {typeof(I).FullName}.");
            }

            return namespaceSegments;
        }
        public static string CompanyName<I>()
        {
            Debug.Assert(typeof(I).IsInterface);
            string[] namespaceSegments = NamespaceSegments<I>();
            return namespaceSegments[Constant.CompanyIndex];
        }

        public static ComponentKeyword ComponentName<I>()
        {
            Debug.Assert(typeof(I).IsInterface);
            string[] namespaceSegments = NamespaceSegments<I>();
            string componentKeyword = namespaceSegments[Constant.ComponentIndex];

            if (!ComponentKeyword.All.Any(keyword => componentKeyword == keyword))
            {
                throw new InvalidOperationException($@"Cannot process component keyword: {componentKeyword}");
            }

            return new ComponentKeyword(componentKeyword);
        }

        public static string VolatilityName<I>()
        {
            Debug.Assert(typeof(I).IsInterface);
            string[] namespaceSegments = NamespaceSegments<I>();
            return namespaceSegments[Constant.VolatilityIndex];
        }

        public static string Microservice<I>()
        {
            Debug.Assert(typeof(I).IsInterface);
            Debug.Assert(
                ComponentName<I>() == ComponentKeyword.Manager,
                $@"Invalid microservice interface. Use only the {ComponentKeyword.Manager} interface to access a microservice.");

            return $@"{CompanyName<I>()}.{ConventionKeyword.Microservice}.{VolatilityName<I>()}";
        }

        public static string Component<I>()
        {
            Debug.Assert(typeof(I).IsInterface);
            return $@"{VolatilityName<I>}{ComponentName<I>()}";
        }

        public static string AppId<I>()
        {
            Debug.Assert(typeof(I).IsInterface);
            return $"{CompanyName<I>()}-{ComponentName<I>()}-{VolatilityName<I>()}-{ConventionKeyword.Service}".ToLowerInvariant();
        }
    }
}