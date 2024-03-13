using Company.iFX.Common;
using NATS.Client.Core;
using NATS.Client.Services;
using ProtoBuf.Grpc;
using System.Diagnostics;
using System.Reflection;
using Zametek.Utility;

namespace Company.iFX.Nats
{
    public abstract class NatsServiceBase
    {
        public async Task AddSubscriberEndpointsAsync<TService>(
            NatsOpts? opts = null,
            NatsSubOpts? subOpts = null,
            NatsPubOpts? pubOpts = null,
            CancellationToken cancellationToken = default) where TService : class
        {
            Type serviceType = typeof(TService);
            serviceType.ThrowIfNotInterface();
            Debug.Assert(GetType().IsAssignableTo(serviceType));

            var taskList = new List<Task>();

            MethodInfo[] interfaceMethods = GetType().GetInterfaceMap(serviceType).TargetMethods;
            MethodInfo subscribeAsyncMethod = typeof(NatsServiceBase).GetMethod(nameof(AddSubscriberEndpointAsync), BindingFlags.Public | BindingFlags.Static)!;

            foreach (MethodInfo interfaceMethodInfo in interfaceMethods)
            {
                ParameterInfo[] parameters = interfaceMethodInfo.GetParameters();

                if (parameters.Length != Constant.NumberOfServiceMethodParameters)
                {
                    throw new InvalidOperationException(
                        $@"Method '{interfaceMethodInfo.Name}' on type '{serviceType.FullName}' must have {Constant.NumberOfServiceMethodParameters} parameters.");
                }

                Type firstParameterType = parameters[0].ParameterType;
                Type returnType = interfaceMethodInfo.ReturnType;

                if (returnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    returnType = returnType.GenericTypeArguments[0]; // Actual return type inside Task.
                }
                else
                {
                    throw new InvalidOperationException(
                        $@"Method '{interfaceMethodInfo.Name}' on type '{serviceType.FullName}' must have a generic Task<> as return type.");
                }

                MethodInfo subscribeAsyncGenericMethod = subscribeAsyncMethod.MakeGenericMethod(serviceType, firstParameterType, returnType);
                Task? methodTask = (Task?)subscribeAsyncGenericMethod.Invoke(this, [this, interfaceMethodInfo, opts, subOpts, pubOpts, cancellationToken]);

                if (methodTask is not null)
                {
                    taskList.Add(methodTask);
                }
            }

            if (taskList.Count == 0)
            {
                throw new InvalidOperationException($@"No methods to host as NATS subscriber endpoints on type '{serviceType.FullName}'.");
            }

            await Task.WhenAll([.. taskList]).ConfigureAwait(false);
        }

        public static async Task AddSubscriberEndpointAsync<TService, TRequest, TReply>(
            NatsServiceBase targetObject,
            MethodInfo methodInfo,
            NatsOpts? opts = null,
            NatsSubOpts? subOpts = null,
            NatsPubOpts? pubOpts = null,
            CancellationToken cancellationToken = default)
            where TService : class
            where TRequest : class
            where TReply : class
        {
            Type serviceType = typeof(TService);
            serviceType.ThrowIfNotInterface();
            ArgumentNullException.ThrowIfNull(methodInfo);

            ParameterInfo[] parameters = methodInfo.GetParameters();
            string methodName = methodInfo.Name;

            if (parameters.Length != Constant.NumberOfServiceMethodParameters)
            {
                throw new InvalidOperationException(
                    $@"Method '{methodName}' on type '{serviceType.FullName}' must have {Constant.NumberOfServiceMethodParameters} parameters.");
            }

            Type secondParameterType = parameters[1].ParameterType;

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
                    serviceType.Namespace);
                activity?.SetTag(
                    Constant.ServiceTypeTag,
                    serviceType.Name);
                activity?.SetTag(
                    Constant.ServiceMethodTag,
                    methodName);

                object token = cancellationToken;

                if (secondParameterType.IsAssignableTo(typeof(CallContext)))
                {
                    token = (CallContext)cancellationToken;
                }

                TReply response = await ((Task<TReply>)methodInfo.Invoke(targetObject, [msg.Data, token])!).ConfigureAwait(false);

                await msg
                    .ReplyAsync(
                        data: response,
                        headers: natsHeaders,
                        serializer: PolymorphicJsonSerializer.Create<TReply?>(),
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
                $@"Failed to subscribe to member '{methodName}' on type '{serviceType.FullName}' with NATS.");
        }

        public async Task AddServiceEndpointsAsync<TService>(
            string serviceVersion,
            string serviceDescription,
            NatsOpts? natsOpts,
            NatsPubOpts? pubOpts = null,
            CancellationToken cancellationToken = default) where TService : class
        {
            Type serviceType = typeof(TService);
            serviceType.ThrowIfNotInterface();
            Debug.Assert(GetType().IsAssignableTo(serviceType));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(serviceVersion);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(serviceDescription);
            ArgumentNullException.ThrowIfNull(natsOpts);

            await using var nats = new NatsConnection(natsOpts ?? NatsOpts.Default);
            var svc = new NatsSvcContext(nats);

            INatsSvcServer service = await svc.AddServiceAsync(new NatsSvcConfig(serviceType.Name, serviceVersion)
            {
                Description = serviceDescription
            }, cancellationToken).ConfigureAwait(false);

            // NatsSvcServer.Group root = await service.AddGroupAsync(
            //     Addressing.Subject<TService>(),
            //     cancellationToken: cancellationToken).ConfigureAwait(false);

            var taskList = new List<Task>();

            MethodInfo[] interfaceMethods = GetType().GetInterfaceMap(serviceType).TargetMethods;
            MethodInfo addEndpointAsyncMethod = typeof(NatsServiceBase).GetMethod(nameof(AddServiceEndpointAsync), BindingFlags.Public | BindingFlags.Static)!;

            foreach (MethodInfo interfaceMethodInfo in interfaceMethods)
            {
                string methodName = interfaceMethodInfo.Name;
                ParameterInfo[] parameters = interfaceMethodInfo.GetParameters();

                if (parameters.Length != Constant.NumberOfServiceMethodParameters)
                {
                    throw new InvalidOperationException(
                        $@"Method '{methodName}' on type '{serviceType.FullName}' must have {Constant.NumberOfServiceMethodParameters} parameters.");
                }

                Type firstParameterType = parameters[0].ParameterType;
                Type returnType = interfaceMethodInfo.ReturnType;

                if (returnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    returnType = returnType.GenericTypeArguments[0]; // Actual return type inside Task.
                }
                else
                {
                    throw new InvalidOperationException(
                        $@"Method '{methodName}' on type '{serviceType.FullName}' must have a generic Task<> as return type.");
                }

                Type cancellationTokenType = parameters[1].ParameterType;

                MethodInfo addEndpointAsyncGenericMethod = addEndpointAsyncMethod.MakeGenericMethod(serviceType, firstParameterType, returnType);
                Task? methodTask = (Task?)addEndpointAsyncGenericMethod.Invoke(
                    this,
                    [this, interfaceMethodInfo, service, cancellationTokenType, pubOpts, cancellationToken]);

                if (methodTask is not null)
                {
                    taskList.Add(methodTask);
                }
            }

            if (taskList.Count == 0)
            {
                throw new InvalidOperationException($@"No methods to host as NATS service endpoints on type '{serviceType.FullName}'.");
            }

            await Task.WhenAll([.. taskList]).ConfigureAwait(false);

            // Await indefinitely.
            await Task.Delay(-1, cancellationToken);
        }

        public static async Task AddServiceEndpointAsync<TService, TRequest, TReply>(
            NatsServiceBase targetObject,
            MethodInfo methodInfo,
            INatsSvcServer root,
            Type cancellationTokenType,
            NatsPubOpts? pubOpts = null,
            CancellationToken cancellationToken = default)
            where TService : class
            where TRequest : class
            where TReply : class
        {
            Type serviceType = typeof(TService);
            serviceType.ThrowIfNotInterface();
            ArgumentNullException.ThrowIfNull(targetObject);
            ArgumentNullException.ThrowIfNull(methodInfo);
            ArgumentNullException.ThrowIfNull(root);
            ArgumentNullException.ThrowIfNull(cancellationTokenType);

            string methodName = methodInfo.Name;

            object token = cancellationToken;
            if (cancellationTokenType.IsAssignableTo(typeof(CallContext)))
            {
                token = (CallContext)cancellationToken;
            }

            async ValueTask Handler(NatsSvcMsg<TRequest> msg)
            {
                FieldInfo natsMsgField = typeof(NatsSvcMsg<TRequest>).GetField(@"_msg", BindingFlags.NonPublic | BindingFlags.Instance)!;
                NatsMsg<TRequest> natsMsg = (NatsMsg<TRequest>)natsMsgField.GetValue(msg)!;

                // Retrieve TrackingContext from headers, or add a
                // TrackingContext to headers if they don't already exist.
                NatsHeaders? natsHeaders = TrackingContextHelper.ProcessHeaders(natsMsg.Headers ?? []);

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
                    serviceType.Namespace);
                activity?.SetTag(
                    Constant.ServiceTypeTag,
                    serviceType.Name);
                activity?.SetTag(
                    Constant.ServiceMethodTag,
                    methodName);

                TReply response = await ((Task<TReply>)methodInfo.Invoke(targetObject, [msg.Data, token])!).ConfigureAwait(false);

                await msg
                    .ReplyAsync(
                        data: response,
                        headers: natsHeaders,
                        serializer: PolymorphicJsonSerializer.Create<TReply?>(),
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
    }
}
