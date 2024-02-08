using System.Diagnostics;
using Zametek.Utility;

namespace Company.iFX.Common
{
    public static class Naming
    {
        public static string[] NamespaceSegments<I>()
        {
            typeof(I).ThrowIfNotInterface();
            string[] namespaceSegments = typeof(I)?.Namespace?.Split('.') ?? Array.Empty<string>();

            if (namespaceSegments.Length != Constant.NamespaceSize)
            {
                throw new FormatException($@"Namespace is an invalid format for type {typeof(I).FullName}.");
            }

            return namespaceSegments;
        }
        public static string CompanyName<I>()
        {
            typeof(I).ThrowIfNotInterface();
            string[] namespaceSegments = NamespaceSegments<I>();
            return namespaceSegments[Constant.CompanyIndex];
        }

        public static ComponentKeyword ComponentName<I>()
        {
            typeof(I).ThrowIfNotInterface();
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
            typeof(I).ThrowIfNotInterface();
            string[] namespaceSegments = NamespaceSegments<I>();
            return namespaceSegments[Constant.VolatilityIndex];
        }

        public static string Microservice<I>()
        {
            typeof(I).ThrowIfNotInterface();
            Debug.Assert(
                ComponentName<I>() == ComponentKeyword.Manager,
                $@"Invalid microservice interface. Use only the {ComponentKeyword.Manager} interface to access a microservice.");

            return $@"{CompanyName<I>()}.{ConventionKeyword.Microservice}.{VolatilityName<I>()}";
        }

        public static string Component<I>()
        {
            typeof(I).ThrowIfNotInterface();
            return $@"{VolatilityName<I>()}{ComponentName<I>()}";
        }

        public static string AppId<I>()
        {
            typeof(I).ThrowIfNotInterface();
            return $"{CompanyName<I>()}-{ComponentName<I>()}-{VolatilityName<I>()}-{ConventionKeyword.Service}".ToLowerInvariant();
        }
    }
}
