using System.Collections.ObjectModel;
using System.Reflection;
using Zametek.Utility;

namespace Company.iFX.Common
{
    public static class SafeEnumStringHelper
    {
        public static ReadOnlyCollection<T> GetAll<T>() where T : SafeEnumString<T>
        {
            return (Assembly.GetAssembly(typeof(T))?
                .GetTypes()
                .Where(t => t.IsClass && (t == typeof(T) || t.IsSubclassOf(typeof(T))))
                .SelectMany(x =>
                    x.GetFields(BindingFlags.Public | BindingFlags.Static)
                        .Select(y => y.GetValue(null) as T)
                        .Where(z => z is not null).Cast<T>())
                .Distinct() ?? Enumerable.Empty<T>())
                .ToList()
                .AsReadOnly();
        }
    }
}
