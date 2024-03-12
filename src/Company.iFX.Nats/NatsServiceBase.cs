using Company.iFX.Common;
using NATS.Client.Core;
using ProtoBuf.Grpc;
using System.Diagnostics;
using System.Reflection;
using Zametek.Utility;

namespace Company.iFX.Nats
{
    public abstract class NatsServiceBase
    {
        public async Task SubscribeAllAsync<TService>(
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
            MethodInfo subscribeAsyncMethod = typeof(NatsServiceBase).GetMethod(nameof(SubscribeAsync), BindingFlags.Public | BindingFlags.Static)!;

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
                throw new InvalidOperationException($@"No methods to host on type '{typeof(TService).FullName}'.");
            }

            await Task.WhenAll([.. taskList]).ConfigureAwait(false);
        }

        public static async Task SubscribeAsync<TService, TRequest, TReply>(
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








        //     public async Task InvokeAllSubscribers<TService>(CancellationToken cancellationToken) where TService : class
        //     {
        //         typeof(TService).ThrowIfNotInterface();
        //         Debug.Assert(GetType().IsAssignableTo(typeof(TService)));

        //         var taskList = new List<Task>();

        //         MethodInfo[] interfaceMethods = GetType().GetInterfaceMap(typeof(TService)).TargetMethods;

        //         foreach (MethodInfo methodInfo in interfaceMethods)
        //         {
        //             ParameterInfo[] parameters = methodInfo.GetParameters();

        //             if (parameters.Length != Constant.NumberOfServiceMethodParameters)
        //             {
        //                 throw new InvalidOperationException(
        //                     $@"Method '{methodInfo.Name}' on type '{typeof(TService).FullName}' must have {Constant.NumberOfServiceMethodParameters} parameters.");
        //             }

        //             Type secondParameterType = parameters[1].ParameterType;
        //             Task? methodTask = null;

        //             if (secondParameterType.IsAssignableTo(typeof(CallContext)))
        //             {
        //                 methodTask = (Task?)methodInfo.Invoke(this, new object[] { null!, (CallContext)cancellationToken });
        //             }
        //             else if (secondParameterType.IsAssignableTo(typeof(CancellationToken)))
        //             {
        //                 methodTask = (Task?)methodInfo.Invoke(this, new object[] { null!, cancellationToken });
        //             }

        //             if (methodTask is not null)
        //             {
        //                 taskList.Add(methodTask);
        //             }
        //         }

        //         if (taskList.Count == 0)
        //         {
        //             throw new InvalidOperationException($@"No methods to host on type '{typeof(TService).FullName}'.");
        //         }

        //         await Task.WhenAll([.. taskList]).ConfigureAwait(false);
        //     }


        // public static async Task SubscribeTypedAsync<TService, TRequest, TReply>(
        //     Func<TRequest?, CancellationToken, Task<TReply?>> func,
        //     NatsOpts? opts = null,
        //     NatsSubOpts? subOpts = null,
        //     NatsPubOpts? pubOpts = null,
        //     [CallerMemberName] string memberName = "",
        //     CancellationToken cancellationToken = default)
        //     where TService : class
        //     where TRequest : class
        //     where TReply : class
        // {
        //     typeof(TService).ThrowIfNotInterface();
        //     ArgumentNullException.ThrowIfNull(func);

        //     await using var nats = new NatsConnection(opts ?? NatsOpts.Default);

        //     Type ServiceType = typeof(TService);

        //     await foreach (NatsMsg<TRequest> msg in nats
        //         .SubscribeAsync(
        //             subject: Addressing.Subject<TService>(memberName),
        //             queueGroup: Addressing.Subject<TService>(),
        //             serializer: PolymorphicJsonSerializer.Create<TRequest>(),
        //             opts: subOpts,
        //             cancellationToken: cancellationToken)
        //         .ConfigureAwait(false))
        //     {
        //         if (cancellationToken.IsCancellationRequested)
        //         {
        //             cancellationToken.ThrowIfCancellationRequested();
        //         }

        //         // Retrieve TrackingContext from headers, or add a
        //         // TrackingContext to headers if they don't already exist.
        //         NatsHeaders natsHeaders = TrackingContextHelper.ProcessHeaders(msg.Headers ?? []);

        //         // NATS does not support OpenTracing yet, so we need to correct for that.
        //         ActivityContext parentContext = OpenTracingHelper.GetParentContext(natsHeaders);

        //         // First activity for an incoming call (i.e. Server kind).
        //         DiagnosticsConfig.NewCurrentIfEmpty<TService>();

        //         using Activity? activity = DiagnosticsConfig.Current.ActivitySource.StartActivity(
        //             memberName,
        //             ActivityKind.Server,
        //             parentContext);

        //         TrackingContext.NewCurrentIfEmpty();

        //         activity?.SetTag(
        //             Constant.TrackingCallChainTag,
        //             TrackingContext.Current.CallChainId.ToDashedString());
        //         activity?.SetTag(
        //             Constant.ServiceNamespaceTag,
        //             ServiceType.Namespace);
        //         activity?.SetTag(
        //             Constant.ServiceTypeTag,
        //             ServiceType.Name);
        //         activity?.SetTag(
        //             Constant.ServiceMethodTag,
        //             memberName);

        //         TReply? response = await func(msg.Data, cancellationToken);

        //         await msg
        //             .ReplyAsync(
        //                 data: response,
        //                 headers: natsHeaders,
        //                 serializer: PolymorphicJsonSerializer.Create<TReply?>(),
        //                 opts: pubOpts,
        //                 cancellationToken: cancellationToken)
        //             .ConfigureAwait(false);

        //         if (cancellationToken.IsCancellationRequested)
        //         {
        //             cancellationToken.ThrowIfCancellationRequested();
        //         }
        //     }

        //     if (cancellationToken.IsCancellationRequested)
        //     {
        //         cancellationToken.ThrowIfCancellationRequested();
        //     }

        //     throw new InvalidOperationException(
        //         $@"Failed to subscribe to member '{memberName}' on type '{typeof(TService).FullName}' with NATS.");
        // }
























        // public static async Task<TReply?> AddEndpointAsync<TService, TRequest, TReply>(
        //     Func<TRequest?, CancellationToken, Task<TReply?>> func,
        //     NatsOpts? opts = null,
        //     NatsSubOpts? subOpts = null,
        //     NatsPubOpts? pubOpts = null,
        //     NatsHeaders? headers = null,
        //     [CallerMemberName] string memberName = "",
        //     CancellationToken cancellationToken = default)
        //     where TService : class
        //     where TRequest : class
        //     where TReply : class
        // {
        //     typeof(TService).ThrowIfNotInterface();
        //     ArgumentNullException.ThrowIfNull(func);

        //     await using var nats = new NatsConnection(opts ?? NatsOpts.Default);

        //     var svc = new NatsSvcContext(nats);

        //     // Retrieve TrackingContext from headers, or add a
        //     // TrackingContext to headers if they don't already exist.
        //     NatsHeaders natsHeaders = TrackingContextHelper.ProcessHeaders(headers ?? []);

        //     Type ServiceType = typeof(TService);








        //     var service = await svc.AddServiceAsync(new NatsSvcConfig(ServiceType.Name, "0.0.1")
        //     {
        //         Description = "Test service"
        //     }, cancellationToken).ConfigureAwait(false);




        //     await service.AddEndpointAsync(
        //         Handler,
        //         subject: Addressing.Subject<TService>(memberName),
        //         queueGroup: Addressing.Subject<TService>(),
        //         serializer: PolymorphicJsonSerializer.Create<TRequest>(),
        //         //opts: subOpts,
        //         cancellationToken: cancellationToken).ConfigureAwait(false);


        //     async ValueTask Handler(NatsSvcMsg<TRequest> msg)
        //     {
        //         if (cancellationToken.IsCancellationRequested)
        //         {
        //             cancellationToken.ThrowIfCancellationRequested();
        //         }


        //         // Retrieve TrackingContext from headers, or add a
        //         // TrackingContext to headers if they don't already exist.
        //         natsHeaders = TrackingContextHelper.ProcessHeaders(msg.Headers ?? []);

        //         // NATS does not support OpenTracing yet, so we need to correct for that.
        //         ActivityContext parentContext = OpenTracingHelper.GetParentContext(natsHeaders);

        //         // First activity for an incoming call (i.e. Server kind).
        //         DiagnosticsConfig.NewCurrentIfEmpty<TService>();

        //         using Activity? activity = DiagnosticsConfig.Current.ActivitySource.StartActivity(
        //             memberName,
        //             ActivityKind.Server,
        //             parentContext);

        //         TrackingContext.NewCurrentIfEmpty();

        //         activity?.SetTag(
        //             Constant.TrackingCallChainTag,
        //             TrackingContext.Current.CallChainId.ToDashedString());
        //         activity?.SetTag(
        //             Constant.ServiceNamespaceTag,
        //             ServiceType.Namespace);
        //         activity?.SetTag(
        //             Constant.ServiceTypeTag,
        //             ServiceType.Name);
        //         activity?.SetTag(
        //             Constant.ServiceMethodTag,
        //             memberName);

        //         TReply? response = await func(msg.Data, cancellationToken);

        //         await msg
        //             .ReplyAsync(
        //                 data: response,
        //                 headers: natsHeaders,
        //                 serializer: PolymorphicJsonSerializer.Create<TReply?>(),
        //                 opts: pubOpts,
        //                 cancellationToken: cancellationToken)
        //             .ConfigureAwait(false);

        //         if (cancellationToken.IsCancellationRequested)
        //         {
        //             cancellationToken.ThrowIfCancellationRequested();
        //         }
        //     }






        //     //var root = await service.AddGroupAsync("minmax");

        //     // await service.AddEndpointAsync(HandleMin, "min", serializer: NATS.Client.Serializers.Json.NatsJsonSerializer<int[]>.Default);
        //     // await service.AddEndpointAsync(HandleMax, "max", serializer: NATS.Client.Serializers.Json.NatsJsonSerializer<int[]>.Default);


        //     // ValueTask HandleMin(NatsSvcMsg<int[]> msg)
        //     // {
        //     //     var min = msg.Data.Min();
        //     //     return msg.ReplyAsync(min);
        //     // }


        //     // ValueTask HandleMax(NatsSvcMsg<int[]> msg)
        //     // {
        //     //     var min = msg.Data.Max();
        //     //     return msg.ReplyAsync(min);
        //     // }




        //     // await foreach (NatsMsg<TRequest> msg in nats
        //     //     .SubscribeAsync(
        //     //         subject: Addressing.Subject<TService>(memberName),
        //     //         queueGroup: Addressing.Subject<TService>(),
        //     //         serializer: PolymorphicJsonSerializer.Create<TRequest>(),
        //     //         opts: subOpts,
        //     //         cancellationToken: cancellationToken)
        //     //     .ConfigureAwait(false))
        //     // {
        //     //     if (cancellationToken.IsCancellationRequested)
        //     //     {
        //     //         cancellationToken.ThrowIfCancellationRequested();
        //     //     }

        //     //     // Retrieve TrackingContext from headers, or add a
        //     //     // TrackingContext to headers if they don't already exist.
        //     //     natsHeaders = TrackingContextHelper.ProcessHeaders(msg.Headers ?? []);

        //     //     // NATS does not support OpenTracing yet, so we need to correct for that.
        //     //     ActivityContext parentContext = OpenTracingHelper.GetParentContext(natsHeaders);

        //     //     // First activity for an incoming call (i.e. Server kind).
        //     //     DiagnosticsConfig.NewCurrentIfEmpty<TService>();

        //     //     using Activity? activity = DiagnosticsConfig.Current.ActivitySource.StartActivity(
        //     //         memberName,
        //     //         ActivityKind.Server,
        //     //         parentContext);

        //     //     TrackingContext.NewCurrentIfEmpty();

        //     //     activity?.SetTag(
        //     //         Constant.TrackingCallChainTag,
        //     //         TrackingContext.Current.CallChainId.ToDashedString());
        //     //     activity?.SetTag(
        //     //         Constant.ServiceNamespaceTag,
        //     //         ServiceType.Namespace);
        //     //     activity?.SetTag(
        //     //         Constant.ServiceTypeTag,
        //     //         ServiceType.Name);
        //     //     activity?.SetTag(
        //     //         Constant.ServiceMethodTag,
        //     //         memberName);

        //     //     TReply? response = await func(msg.Data, cancellationToken);

        //     //     await msg
        //     //         .ReplyAsync(
        //     //             data: response,
        //     //             headers: natsHeaders,
        //     //             serializer: PolymorphicJsonSerializer.Create<TReply?>(),
        //     //             opts: pubOpts,
        //     //             cancellationToken: cancellationToken)
        //     //         .ConfigureAwait(false);

        //     //     if (cancellationToken.IsCancellationRequested)
        //     //     {
        //     //         cancellationToken.ThrowIfCancellationRequested();
        //     //     }
        //     // }

        //     // if (cancellationToken.IsCancellationRequested)
        //     // {
        //     //     cancellationToken.ThrowIfCancellationRequested();
        //     // }

        //     throw new InvalidOperationException(
        //         $@"Failed to subscribe to member '{memberName}' on type '{typeof(TService).FullName}' with NATS.");
        // }

    }
}
