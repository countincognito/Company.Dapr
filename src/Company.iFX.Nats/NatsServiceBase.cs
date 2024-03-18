using Company.iFX.Common;
using NATS.Client.Core;
using NATS.Client.Services;
using ProtoBuf.Grpc;
using System.Diagnostics;
using System.Reflection;
using Zametek.Utility;

namespace Company.iFX.Nats
{
    public abstract class NatsServiceBase<TService> where TService : class
    {
        private static readonly Type s_ServiceType;
        private static readonly MethodInfo s_SubscribeAsyncMethod;
        private static readonly MethodInfo s_AddEndpointAsyncMethod;

        private readonly MethodInfo[] m_InterfaceMethods;

        static NatsServiceBase()
        {
            s_ServiceType = typeof(TService);
            s_ServiceType.ThrowIfNotInterface();

            s_SubscribeAsyncMethod = typeof(NatsServiceBase<TService>)
                .GetMethod(
                    nameof(AddSubscriberEndpointAsync),
                    BindingFlags.Public | BindingFlags.Static)!;
            ArgumentNullException.ThrowIfNull(s_SubscribeAsyncMethod);

            s_AddEndpointAsyncMethod = typeof(NatsServiceBase<TService>)
                .GetMethod(
                    nameof(AddServiceEndpointAsync),
                    BindingFlags.Public | BindingFlags.Static)!;
            ArgumentNullException.ThrowIfNull(s_AddEndpointAsyncMethod);
        }

        public NatsServiceBase()
        {
            Debug.Assert(GetType().IsAssignableTo(s_ServiceType));
            m_InterfaceMethods = GetType().GetInterfaceMap(s_ServiceType).TargetMethods;
            ArgumentNullException.ThrowIfNull(m_InterfaceMethods);

            if (m_InterfaceMethods.Length == 0)
            {
                throw new InvalidOperationException($@"No methods to host as NATS subscriber endpoints on type '{s_ServiceType.FullName}'.");
            }
        }

        public async Task AddSubscriberEndpointsAsync(
            NatsOpts? opts = null,
            NatsSubOpts? subOpts = null,
            NatsPubOpts? pubOpts = null,
            CancellationToken cancellationToken = default)
        {
            var taskList = new List<Task>();

            foreach (MethodInfo methodInfo in m_InterfaceMethods)
            {
                (Type firstParameterType, Type secondParameterType, Type returnType) = ValidateMethodParameters(methodInfo);

                MethodInfo subscribeAsyncGenericMethod = s_SubscribeAsyncMethod.MakeGenericMethod(firstParameterType, returnType);
                Task? methodTask = (Task?)subscribeAsyncGenericMethod.Invoke(this, [this, methodInfo, opts, subOpts, pubOpts, cancellationToken]);

                if (methodTask is not null)
                {
                    taskList.Add(methodTask);
                }
            }

            if (taskList.Count == 0)
            {
                throw new InvalidOperationException($@"No methods to host as NATS subscriber endpoints on type '{s_ServiceType.FullName}'.");
            }

            await Task.WhenAll([.. taskList]).ConfigureAwait(false);
        }

        public static async Task AddSubscriberEndpointAsync<TRequest, TReply>(
            NatsServiceBase<TService> targetObject,
            MethodInfo methodInfo,
            NatsOpts? opts = null,
            NatsSubOpts? subOpts = null,
            NatsPubOpts? pubOpts = null,
            CancellationToken cancellationToken = default)
            where TRequest : class
            where TReply : class
        {
            string methodName = methodInfo.Name;

            (_, Type secondParameterType, _) = ValidateMethodParameters(methodInfo);

            object token = cancellationToken;
            if (secondParameterType.IsAssignableTo(typeof(CallContext)))
            {
                token = (CallContext)cancellationToken;
            }

            await using var nats = new NatsConnection(opts ?? NatsOpts.Default);

            await foreach (NatsMsg<TRequest> msg in nats
                .SubscribeAsync(
                    subject: Addressing.Subject<TService>(methodName),
                    queueGroup: Addressing.Subject<TService>(),
                    serializer: PolymorphicJsonSerializer.Create<TRequest>(),
                    opts: subOpts,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                // Retrieve TrackingContext from headers, or add a
                // TrackingContext to headers if they don't already exist.
                NatsHeaders natsHeaders = TrackingContextHelper.ProcessHeaders(msg.Headers ?? []);

                // NATS does not support OpenTracing yet, so we need to correct for that.
                ActivityContext parentContext = OpenTracingHelper.GetParentContext(natsHeaders);

                // First activity for an incoming call (i.e. Server kind).
                DiagnosticsConfig.NewCurrentIfEmpty<TService>();

                using Activity? activity = DiagnosticsConfig.Current.ActivitySource.StartActivity(
                    methodName,
                    ActivityKind.Server,
                    parentContext);

                TrackingContext.NewCurrentIfEmpty();

                activity?.SetTag(
                    Constant.TrackingCallChainTag,
                    TrackingContext.Current.CallChainId.ToDashedString());
                activity?.SetTag(
                    Constant.ServiceNamespaceTag,
                    s_ServiceType.Namespace);
                activity?.SetTag(
                    Constant.ServiceTypeTag,
                    s_ServiceType.Name);
                activity?.SetTag(
                    Constant.ServiceMethodTag,
                    methodName);

                TReply response = await ((Task<TReply>)methodInfo.Invoke(targetObject, [msg.Data, token])!).ConfigureAwait(false);

                await msg
                    .ReplyAsync(
                        data: response,
                        headers: natsHeaders,
                        serializer: PolymorphicJsonSerializer.Create<TReply>(),
                        opts: pubOpts,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            throw new InvalidOperationException(
                $@"Failed to subscribe to member '{methodName}' on type '{s_ServiceType.FullName}' with NATS.");
        }

        public async Task AddServiceEndpointsAsync(
            string serviceVersion,
            string serviceDescription,
            NatsOpts? natsOpts,
            NatsPubOpts? pubOpts = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(serviceVersion);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(serviceDescription);
            ArgumentNullException.ThrowIfNull(natsOpts);

            await using var nats = new NatsConnection(natsOpts ?? NatsOpts.Default);
            var svc = new NatsSvcContext(nats);

            INatsSvcServer service = await svc.AddServiceAsync(new NatsSvcConfig(s_ServiceType.Name, serviceVersion)
            {
                Description = serviceDescription
            }, cancellationToken).ConfigureAwait(false);

            // NatsSvcServer.Group root = await service.AddGroupAsync(
            //     Addressing.Subject<TService>(),
            //     cancellationToken: cancellationToken).ConfigureAwait(false);

            var taskList = new List<Task>();

            foreach (MethodInfo methodInfo in m_InterfaceMethods)
            {
                (Type firstParameterType, _, Type returnType) = ValidateMethodParameters(methodInfo);

                MethodInfo addEndpointAsyncGenericMethod = s_AddEndpointAsyncMethod.MakeGenericMethod(firstParameterType, returnType);
                Task? methodTask = (Task?)addEndpointAsyncGenericMethod.Invoke(
                    this,
                    [this, methodInfo, service, pubOpts, cancellationToken]);

                if (methodTask is not null)
                {
                    taskList.Add(methodTask);
                }
            }

            if (taskList.Count == 0)
            {
                throw new InvalidOperationException($@"No methods to host as NATS service endpoints on type '{s_ServiceType.FullName}'.");
            }

            await Task.WhenAll([.. taskList]).ConfigureAwait(false);

            // Await indefinitely.
            await Task.Delay(-1, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public static async Task AddServiceEndpointAsync<TRequest, TReply>(
            NatsServiceBase<TService> targetObject,
            MethodInfo methodInfo,
            INatsSvcServer root,
            NatsPubOpts? pubOpts = null,
            CancellationToken cancellationToken = default)
            where TRequest : class
            where TReply : class
        {
            ArgumentNullException.ThrowIfNull(targetObject);
            ArgumentNullException.ThrowIfNull(methodInfo);
            ArgumentNullException.ThrowIfNull(root);

            string methodName = methodInfo.Name;

            (_, Type secondParameterType, _) = ValidateMethodParameters(methodInfo);

            object token = cancellationToken;
            if (secondParameterType.IsAssignableTo(typeof(CallContext)))
            {
                token = (CallContext)cancellationToken;
            }

            FieldInfo? natsMsgField = typeof(NatsSvcMsg<TRequest>).GetField(@"_msg", BindingFlags.NonPublic | BindingFlags.Instance);
            ArgumentNullException.ThrowIfNull(natsMsgField);

            async ValueTask Handler(NatsSvcMsg<TRequest> msg)
            {
                NatsMsg<TRequest>? natsMsg = natsMsgField.GetValue(msg) as NatsMsg<TRequest>?;
                ArgumentNullException.ThrowIfNull(natsMsg);

                // Retrieve TrackingContext from headers, or add a
                // TrackingContext to headers if they don't already exist.
                NatsHeaders? natsHeaders = TrackingContextHelper.ProcessHeaders(natsMsg?.Headers ?? []);

                // NATS does not support OpenTracing yet, so we need to correct for that.
                ActivityContext parentContext = OpenTracingHelper.GetParentContext(natsHeaders);

                // First activity for an incoming call (i.e. Server kind).
                DiagnosticsConfig.NewCurrentIfEmpty<TService>();

                using Activity? activity = DiagnosticsConfig.Current.ActivitySource.StartActivity(
                    methodName,
                    ActivityKind.Server,
                    parentContext);

                TrackingContext.NewCurrentIfEmpty();

                activity?.SetTag(
                    Constant.TrackingCallChainTag,
                    TrackingContext.Current.CallChainId.ToDashedString());
                activity?.SetTag(
                    Constant.ServiceNamespaceTag,
                    s_ServiceType.Namespace);
                activity?.SetTag(
                    Constant.ServiceTypeTag,
                    s_ServiceType.Name);
                activity?.SetTag(
                    Constant.ServiceMethodTag,
                    methodName);

                TReply response = await ((Task<TReply>)methodInfo.Invoke(targetObject, [msg.Data, token])!).ConfigureAwait(false);

                await msg
                    .ReplyAsync(
                        data: response,
                        headers: natsHeaders,
                        serializer: PolymorphicJsonSerializer.Create<TReply>(),
                        opts: pubOpts,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }

            await root.AddEndpointAsync(
                Handler,
                name: methodName,
                subject: Addressing.Subject<TService>(methodName),
                serializer: PolymorphicJsonSerializer.Create<TRequest>(),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public static (Type, Type, Type) ValidateMethodParameters(MethodInfo methodInfo)
        {
            ArgumentNullException.ThrowIfNull(methodInfo);
            string methodName = methodInfo.Name;

            ParameterInfo[] parameters = methodInfo.GetParameters();

            if (parameters.Length != Constant.NumberOfServiceMethodParameters)
            {
                throw new InvalidOperationException(
                    $@"Method '{methodName}' on type '{s_ServiceType.FullName}' must have {Constant.NumberOfServiceMethodParameters} parameters.");
            }

            Type firstParameterType = parameters[0].ParameterType;

            if (!firstParameterType.CanSerialize())
            {
                throw new InvalidOperationException(
                    $@"First parameter of type {firstParameterType.FullName} on Method '{methodName}' on type '{s_ServiceType.FullName}' must be serializable.");
            }

            Type returnType = methodInfo.ReturnType;

            if (returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GenericTypeArguments[0]; // Actual return type inside Task.
            }
            else
            {
                throw new InvalidOperationException(
                    $@"Method '{methodName}' on type '{s_ServiceType.FullName}' must have a generic Task<> as return type.");
            }

            if (!returnType.CanSerialize())
            {
                throw new InvalidOperationException(
                    $@"Return type {returnType.FullName} on Method '{methodName}' on type '{s_ServiceType.FullName}' must be serializable.");
            }

            Type secondParameterType = parameters[1].ParameterType;

            if (secondParameterType != typeof(CancellationToken)
                && secondParameterType != typeof(CallContext))
            {
                throw new InvalidOperationException(
                    $@"Second parameter on Method '{methodName}' on type '{s_ServiceType.FullName}' must be either CancellationToken or CallContext.");
            }

            return (firstParameterType, secondParameterType, returnType);
        }
    }
}
