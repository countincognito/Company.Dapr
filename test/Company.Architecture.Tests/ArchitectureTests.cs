using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Company.iFX.Common;
using System.Diagnostics;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Company.Architecture.Tests
{
    public class ArchitectureTests
    {
        private static readonly string s_CompanyName = @"Company";
        private static readonly string s_Membership = @"Membership";
        private static readonly string s_Registration = @"Registration";
        private static readonly string s_User = @"User";
        private static readonly string s_Encryption = @"Encryption";
        private static readonly string s_Cache = @"Cache";

        private static readonly ArchUnitNET.Domain.Architecture s_Architecture;

        private static readonly IObjectProvider<IType> s_ManagerLayer =
            Types().That()
            .ResideInNamespace($@"{s_CompanyName}\.{ComponentKeyword.Manager}\..+", true)
            .As($@"{ComponentKeyword.Manager} Layer");

        private static readonly IObjectProvider<IType> s_MembershipManagerLayer =
            Types().That()
            .ResideInNamespace($@"{s_CompanyName}\.{ComponentKeyword.Manager}\.{s_Membership}\..+", true)
            .As($@"{s_Membership}{ComponentKeyword.Manager} Layer");

        private static readonly IObjectProvider<IType> s_EngineLayer =
            Types().That()
            .ResideInNamespace($@"{s_CompanyName}\.{ComponentKeyword.Engine}\..+", true)
            .As($@"{ComponentKeyword.Engine} Layer");

        private static readonly IObjectProvider<IType> s_RegistrationEngineLayer =
            Types().That()
            .ResideInNamespace($@"{s_CompanyName}\.{ComponentKeyword.Engine}\.{s_Registration}\..+", true)
            .As($@"{s_Registration}{ComponentKeyword.Engine} Layer");

        private static readonly IObjectProvider<IType> s_AccessLayer =
            Types().That()
            .ResideInNamespace($@"{s_CompanyName}\.{ComponentKeyword.Access}\..+", true)
            .As($@"{ComponentKeyword.Access} Layer");

        private static readonly IObjectProvider<IType> s_UserAccessLayer =
            Types().That()
            .ResideInNamespace($@"{s_CompanyName}\.{ComponentKeyword.Access}\.{s_User}\..+", true)
            .As($@"{s_User}{ComponentKeyword.Access} Layer");

        private static readonly IObjectProvider<IType> s_UtilityLayer =
            Types().That()
            .ResideInNamespace($@"{s_CompanyName}\.{ComponentKeyword.Utility}\..+", true)
            .As($@"{ComponentKeyword.Utility} Layer");

        private static readonly IObjectProvider<IType> s_EncryptionUtilityLayer =
            Types().That()
            .ResideInNamespace($@"{s_CompanyName}\.{ComponentKeyword.Utility}\.{s_Encryption}\..+", true)
            .As($@"{s_Encryption}{ComponentKeyword.Utility} Layer");

        private static readonly IObjectProvider<IType> s_CacheUtilityLayer =
            Types().That()
            .ResideInNamespace($@"{s_CompanyName}\.{ComponentKeyword.Utility}\.{s_Cache}\..+", true)
            .As($@"{s_Cache}{ComponentKeyword.Utility} Layer");

        //private static readonly IObjectProvider<IType> s_iFXLayer =
        //    Types().That()
        //    .ResideInNamespace($@"{s_CompanyName}\.iFX\..+", true)
        //    .As($@"iFX Layer");

        static ArchitectureTests()
        {
            s_Architecture = new ArchLoader().LoadAssemblies(GetAssemblies(s_CompanyName)).Build();
        }

        // Managers

        private static System.Reflection.Assembly[] GetAssemblies(string companyName)
        {
            string? path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string[] assemblyNames = Directory.GetFiles(
                path!,
                $@"{companyName}.*.{ConventionKeyword.Service}.dll",
                SearchOption.TopDirectoryOnly);

            var assemblyList = new List<System.Reflection.Assembly>();

            foreach (string? assemblyName in assemblyNames)
            {
                Debug.Assert(assemblyName is not null);
                var assembly = System.Reflection.Assembly.LoadFrom(assemblyName);

                assemblyList.Add(assembly);
            }

            return assemblyList.ToArray();
        }

        [Fact]
        public void ArchitectureTests_GivenManagerLayer_ThenMustNotReferenceOtherManagers()
        {
            IEnumerable<IType> allManagerTypes = s_ManagerLayer.GetObjects(s_Architecture);

            IEnumerable<IType> membershipManagerTypes = s_MembershipManagerLayer.GetObjects(s_Architecture);
            IEnumerable<IType> managerTypesOtherThanMembership = allManagerTypes.Except(membershipManagerTypes);

            IArchRule rule =
                Types().That().Are(membershipManagerTypes).Should()
                .NotDependOnAny(managerTypesOtherThanMembership)
                .Because($@"{s_Membership}{ComponentKeyword.Manager} should not reference another {ComponentKeyword.Manager}.");

            // TODO add more manager types when they exist

            IArchRule combinedRule = rule;

            combinedRule.Check(s_Architecture);
        }

        // Engines

        [Fact]
        public void ArchitectureTests_GivenEngineLayer_ThenMustNotReferenceOtherEngines()
        {
            IEnumerable<IType> allEngineTypes = s_EngineLayer.GetObjects(s_Architecture);

            IEnumerable<IType> registrationEngineTypes = s_RegistrationEngineLayer.GetObjects(s_Architecture);
            IEnumerable<IType> engineTypesOtherThanRegistration = allEngineTypes.Except(registrationEngineTypes);

            IArchRule rule =
                Types().That().Are(registrationEngineTypes).Should()
                .NotDependOnAny(engineTypesOtherThanRegistration)
                .Because($@"{s_Registration}{ComponentKeyword.Engine} should not reference another {ComponentKeyword.Engine}.");

            // TODO add more engine types when they exist

            IArchRule combinedRule = rule;

            combinedRule.Check(s_Architecture);
        }

        [Fact]
        public void ArchitectureTests_GivenEngineLayer_ThenMustNotReferenceManagers()
        {
            IEnumerable<IType> allEngineTypes = s_EngineLayer.GetObjects(s_Architecture);
            IEnumerable<IType> allManagerTypes = s_ManagerLayer.GetObjects(s_Architecture);

            IArchRule rule =
                Types().That().Are(allEngineTypes).Should()
                .NotDependOnAny(allManagerTypes)
                .Because($@"{ComponentKeyword.Engine} should not reference {ComponentKeyword.Manager}.");

            IArchRule combinedRule = rule;

            combinedRule.Check(s_Architecture);
        }

        // Access

        [Fact]
        public void ArchitectureTests_GivenAccessLayer_ThenMustNotReferenceOtherAccesses()
        {
            IEnumerable<IType> allAccessTypes = s_AccessLayer.GetObjects(s_Architecture);

            IEnumerable<IType> userAccessTypes = s_UserAccessLayer.GetObjects(s_Architecture);
            IEnumerable<IType> accessTypesOtherThanUser = allAccessTypes.Except(userAccessTypes);

            IArchRule rule =
                Types().That().Are(userAccessTypes).Should()
                .NotDependOnAny(accessTypesOtherThanUser)
                .Because($@"{s_User}{ComponentKeyword.Access} should not reference another {ComponentKeyword.Access}.");

            // TODO add more access types when they exist

            IArchRule combinedRule = rule;

            combinedRule.Check(s_Architecture);
        }

        [Fact]
        public void ArchitectureTests_GivenAccessLayer_ThenMustNotReferenceManagers()
        {
            IEnumerable<IType> allAccessTypes = s_AccessLayer.GetObjects(s_Architecture);
            IEnumerable<IType> allManagerTypes = s_ManagerLayer.GetObjects(s_Architecture);

            IArchRule rule =
                Types().That().Are(allAccessTypes).Should()
                .NotDependOnAny(allManagerTypes)
                .Because($@"{ComponentKeyword.Access} should not reference {ComponentKeyword.Manager}.");

            IArchRule combinedRule = rule;

            combinedRule.Check(s_Architecture);
        }

        [Fact]
        public void ArchitectureTests_GivenAccessLayer_ThenMustNotReferenceEngines()
        {
            IEnumerable<IType> allAccessTypes = s_AccessLayer.GetObjects(s_Architecture);
            IEnumerable<IType> allEngineTypes = s_EngineLayer.GetObjects(s_Architecture);

            IArchRule rule =
                Types().That().Are(allAccessTypes).Should()
                .NotDependOnAny(allEngineTypes)
                .Because($@"{ComponentKeyword.Access} should not reference {ComponentKeyword.Engine}.");

            IArchRule combinedRule = rule;

            combinedRule.Check(s_Architecture);
        }

        // Utility

        [Fact]
        public void ArchitectureTests_GivenUtilityLayer_ThenMustNotReferenceOtherUtilities()
        {
            IEnumerable<IType> allUtilityTypes = s_UtilityLayer.GetObjects(s_Architecture);

            IEnumerable<IType> encryptionUtilityTypes = s_EncryptionUtilityLayer.GetObjects(s_Architecture);
            IEnumerable<IType> utilityTypesOtherThanEncryption = allUtilityTypes.Except(encryptionUtilityTypes);

            IEnumerable<IType> cacheUtilityTypes = s_CacheUtilityLayer.GetObjects(s_Architecture);
            IEnumerable<IType> utilityTypesOtherThanCache = allUtilityTypes.Except(cacheUtilityTypes);

            IArchRule rule1 =
                Types().That().Are(encryptionUtilityTypes).Should()
                .NotDependOnAny(utilityTypesOtherThanEncryption)
                .Because($@"{s_Encryption}{ComponentKeyword.Utility} should not reference another {ComponentKeyword.Utility}.");

            IArchRule rule2 =
                Types().That().Are(cacheUtilityTypes).Should()
                .NotDependOnAny(utilityTypesOtherThanCache)
                .Because($@"{s_Cache}{ComponentKeyword.Utility} should not reference another {ComponentKeyword.Utility}.");

            // TODO add more access types when they exist

            IArchRule combinedRule = rule1.And(rule2);

            combinedRule.Check(s_Architecture);
        }

        [Fact]
        public void ArchitectureTests_GivenUtilityLayer_ThenMustNotReferenceManagers()
        {
            IEnumerable<IType> allUtilityTypes = s_UtilityLayer.GetObjects(s_Architecture);
            IEnumerable<IType> allManagerTypes = s_ManagerLayer.GetObjects(s_Architecture);

            IArchRule rule =
                Types().That().Are(allUtilityTypes).Should()
                .NotDependOnAny(allManagerTypes)
                .Because($@"{ComponentKeyword.Utility} should not reference {ComponentKeyword.Manager}.");

            IArchRule combinedRule = rule;

            combinedRule.Check(s_Architecture);
        }

        [Fact]
        public void ArchitectureTests_GivenUtilityLayer_ThenMustNotReferenceEngines()
        {
            IEnumerable<IType> allUtilityTypes = s_UtilityLayer.GetObjects(s_Architecture);
            IEnumerable<IType> allEngineTypes = s_EngineLayer.GetObjects(s_Architecture);

            IArchRule rule =
                Types().That().Are(allUtilityTypes).Should()
                .NotDependOnAny(allEngineTypes)
                .Because($@"{ComponentKeyword.Utility} should not reference {ComponentKeyword.Engine}.");

            IArchRule combinedRule = rule;

            combinedRule.Check(s_Architecture);
        }

        [Fact]
        public void ArchitectureTests_GivenUtilityLayer_ThenMustNotReferenceAccesses()
        {
            IEnumerable<IType> allUtilityTypes = s_UtilityLayer.GetObjects(s_Architecture);
            IEnumerable<IType> allAccessTypes = s_AccessLayer.GetObjects(s_Architecture);

            IArchRule rule =
                Types().That().Are(allUtilityTypes).Should()
                .NotDependOnAny(allAccessTypes)
                .Because($@"{ComponentKeyword.Utility} should not reference {ComponentKeyword.Access}.");

            IArchRule combinedRule = rule;

            combinedRule.Check(s_Architecture);
        }
    }
}