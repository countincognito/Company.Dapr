using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Company.iFX.Common
{
    public class PolymorphicTypeResolver
        : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(
            Type type,
            JsonSerializerOptions options)
        {
            JsonTypeInfo typeInfo = base.GetTypeInfo(type, options);

            if (type.IsAbstract && type.IsClass)
            {
                Type[] assemblyTypes = typeInfo.Type.Assembly.GetTypes();

                bool isDerivedTypePredicate(Type type)
                {
                    bool isDerivedType = !type.IsAbstract && type.IsClass && typeInfo.Type.IsAssignableFrom(type);
                    return isDerivedType;
                }

                JsonDerivedType derivedTypeSelector(Type type)
                {
                    Debug.Assert(type.FullName is not null);
                    JsonDerivedType derivedType = new(type, type.FullName);
                    return derivedType;
                }

                List<JsonDerivedType> derivedTypes = assemblyTypes
                    .Where(isDerivedTypePredicate)
                    .Select(derivedTypeSelector)
                    .ToList();

                if (derivedTypes.Any())
                {
                    typeInfo.PolymorphismOptions = new JsonPolymorphismOptions();

                    foreach (JsonDerivedType derivedType in derivedTypes)
                    {
                        typeInfo.PolymorphismOptions.DerivedTypes.Add(derivedType);
                    }
                }
            }
            // Workaround for https://github.com/dotnet/aspnetcore/issues/44852
            // PR with a fix was merged in https://github.com/dotnet/aspnetcore/pull/45405
            // But it won't arrive until aspnetcore 8.0
            else if (!type.IsAbstract
                && type.IsClass
                && type.BaseType is not null
                && type.BaseType.IsAbstract
                && type.BaseType.IsClass
                && typeInfo.Kind != JsonTypeInfoKind.None)
            {
                Debug.Assert(type.FullName is not null);
                JsonDerivedType derivedType = new(type, type.FullName);

                typeInfo.PolymorphismOptions = new JsonPolymorphismOptions();
                typeInfo.PolymorphismOptions.DerivedTypes.Add(derivedType);
            }

            return typeInfo;
        }
    }
}
