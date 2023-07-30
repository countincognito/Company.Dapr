using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using Company.iFX.Common;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Reflection;

namespace Company.iFX.Container
{
    public static class Container
    {
        private static ILifetimeScope? s_Scope;
        private static ILifetimeScope? s_MainTestScope;

        public static TService GetService<TService>()
            where TService : notnull
        {
            if (s_Scope is null)
            {
                throw new InvalidOperationException("Lifetime scope is null");
            }

            return s_Scope.Resolve<TService>();
        }

        public static object GetService(Type serviceType)
        {
            if (s_Scope is null)
            {
                throw new InvalidOperationException("Lifetime scope is null");
            }

            return s_Scope.Resolve(serviceType);
        }

        public static IHostBuilder UseiFX(
            this IHostBuilder hostBuilder,
            string companyName = "*")
        {
            if (Configuration.Configuration.SystemUnderTest)
            {
                throw new InvalidOperationException("Cannot use iFX when system is under test");
            }

            var factoryProvider = new AutofacServiceProviderFactory(builder =>
            {
                builder.RegisterBuildCallback(scope => s_Scope = scope);
                LoadAssemblies(companyName, builder);
                LoadMapper(companyName, builder);
            });
            return hostBuilder.UseServiceProviderFactory(factoryProvider);
        }

        public static void ConfigureTesting(
            object logger,
            Type[] types)
        {
            if (!Configuration.Configuration.SystemUnderTest)
            {
                throw new InvalidOperationException("System must be under test to configure testing");
            }

            BuildTestContainer(logger, types);
        }

        public static void CreateTestScope(object[] mocks)
        {
            if (!Configuration.Configuration.SystemUnderTest)
            {
                return;
            }

            ILifetimeScope? testScope = s_MainTestScope?.BeginLifetimeScope(configuration =>
            {
                foreach (object mock in mocks)
                {
                    Type interfaceType = mock.GetType().GetInterfaces().First();
                    configuration.RegisterInstance(mock).As(interfaceType);
                }
            });

            OverrideScope(testScope);
        }

        public static void EndTestScope()
        {
            if (!Configuration.Configuration.SystemUnderTest)
            {
                return;
            }

            EndScope();
        }

        public static void EndScope()
        {
            s_Scope?.Dispose();
        }

        public static void OverrideScope(ILifetimeScope? scope)
        {
            s_Scope = scope;
        }

        private static void LoadAssemblies(
            string companyName,
            ContainerBuilder builder)
        {
            string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string[] assemblyNames = Directory.GetFiles(
                path!,
                $@"{companyName}.*.{ConventionKeyword.Impl}.dll",
                SearchOption.TopDirectoryOnly);

            foreach (string? assemblyName in assemblyNames)
            {
                Debug.Assert(assemblyName is not null);
                builder.RegisterAssemblyTypes(Assembly.LoadFrom(assemblyName))
                    .Where(type => ComponentKeyword.All.Any(keyword => type.Name.EndsWith(keyword)))
                    .As(type => type.GetInterfaces());
            }
        }

        private static void LoadMapper(
            string companyName,
            ContainerBuilder builder)
        {
            string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string[] assemblyNames = Directory.GetFiles(
                path!,
                $@"{companyName}.*.dll",
                SearchOption.TopDirectoryOnly);

            foreach (string? assemblyName in assemblyNames)
            {
                Debug.Assert(assemblyName is not null);

                Assembly assembly = Assembly.LoadFrom(assemblyName);

                IList<Type> assembliesTypes = assembly
                    .GetTypes()
                    .Where(type => typeof(Profile).IsAssignableFrom(type) && type.IsPublic && !type.IsAbstract)
                    .Distinct()
                    .ToList();

                //IList<Profile> autoMapperProfiles = assembliesTypes
                //    .Select(type => (Profile)Activator.CreateInstance(type)!)
                //    .ToList();

                if (assembliesTypes.Any())
                {
                    builder.RegisterAutoMapper(assembly);
                }

                //builder.Register(ctx => new MapperConfiguration(cfg =>
                //{
                //    foreach (Type assembliesType in assembliesTypes)
                //    {
                //        Profile profile = (Profile)Activator.CreateInstance(assembliesType)!;
                //        cfg.AddProfile(profile);
                //    }

                //    //foreach (var profile in autoMapperProfiles)
                //    //{
                //    //    cfg.AddProfile(profile);
                //    //}
                //}));
            }

            // TODO : check this works in Under Test
            //builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper())
            //    .As<IMapper>()
            //    .InstancePerLifetimeScope();
        }

        private static void BuildTestContainer(
            object logger,
            Type[] types)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(logger).As(logger.GetType().GetInterfaces());
            builder.RegisterTypes(types).As(t => t.GetInterfaces());
            s_MainTestScope = builder.Build().BeginLifetimeScope();
        }
    }
}
