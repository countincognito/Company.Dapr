﻿using Company.iFX.Common;
using NATS.Client.Core;
using ProtoBuf.Grpc;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Zametek.Utility;

namespace Company.iFX.Nats
{
    public abstract class NatsServiceBase
    {
        public async Task Invoke<TService>(CancellationToken cancellationToken) where TService : class
        {
            typeof(TService).ThrowIfNotInterface();
            Debug.Assert(GetType().IsAssignableTo(typeof(TService)));

            var taskList = new List<Task>();

            MethodInfo[] interfaceMethods = GetType().GetInterfaceMap(typeof(TService)).TargetMethods;

            foreach (MethodInfo methodInfo in interfaceMethods)
            {
                ParameterInfo[] parameters = methodInfo.GetParameters();

                if (parameters.Length != Constant.NumberOfServiceMethodParameters)
                {
                    throw new InvalidOperationException(
                        $@"Method '{methodInfo.Name}' on type '{typeof(TService).FullName}' must have {Constant.NumberOfServiceMethodParameters} parameters.");
                }

                Type secondParameterType = parameters[1].ParameterType;
                Task? methodTask = null;

                if (secondParameterType.IsAssignableTo(typeof(CallContext)))
                {
                    methodTask = (Task?)methodInfo.Invoke(this, new object[] { null!, (CallContext)cancellationToken });
                }
                else if (secondParameterType.IsAssignableTo(typeof(CancellationToken)))
                {
                    methodTask = (Task?)methodInfo.Invoke(this, new object[] { null!, cancellationToken });
                }

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

        public static async Task<TReply?> SubscribeAsync<TService, TRequest, TReply>(
            Func<TRequest?, CancellationToken, Task<TReply?>> func,
            NatsOpts? opts = null,
            NatsSubOpts? subOpts = null,
            NatsPubOpts? pubOpts = null,
            NatsHeaders? headers = null,
            [CallerMemberName] string memberName = "",
            CancellationToken cancellationToken = default)
            where TService : class
            where TRequest : class
            where TReply : class
        {
            typeof(TService).ThrowIfNotInterface();
            ArgumentNullException.ThrowIfNull(func);

            await using var nats = new NatsConnection(opts ?? NatsOpts.Default);

            // Retrieve TrackingContext from headers.
            NatsHeaders natsHeaders = TrackingContextHelper.ProcessHeaders(headers ?? []);

            await foreach (NatsMsg<TRequest> msg in nats
                .SubscribeAsync(
                    subject: Addressing.Subject<TService>(memberName),
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

                // Retrieve TrackingContext from headers.
                natsHeaders = TrackingContextHelper.ProcessHeaders(msg.Headers ?? []);

                TReply? response = await func(msg.Data, cancellationToken);

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
                $@"Failed to subscribe to member '{memberName}' on type '{typeof(TService).FullName}' with NATS.");
        }
    }
}
