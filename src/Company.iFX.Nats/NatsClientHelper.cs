using NATS.Client.Core;
using System.Runtime.CompilerServices;
using Zametek.Utility;

namespace Company.iFX.Nats
{
    public static class NatsClientHelper
    {
        public static async Task<TReply?> CallAsync<TService, TRequest, TReply>(
            TRequest request,
            NatsOpts? opts = null,
            NatsHeaders? headers = null,
            [CallerMemberName] string memberName = "",
            CancellationToken cancellationToken = default)
            where TService : class
            where TRequest : class
            where TReply : class
        {
            typeof(TService).ThrowIfNotInterface();
            ArgumentNullException.ThrowIfNull(request);

            await using var nats = new NatsConnection(opts ?? NatsOpts.Default);

            // Retrieve TrackingContext from headers, or add a
            // TrackingContext to headers if they don't already exist.
            NatsHeaders natsHeaders = TrackingContextHelper.ProcessHeaders(headers ?? []);

            // NATS does not support OpenTracing yet, so we need to correct for that.
            natsHeaders = OpenTracingHelper.ProcessClientHeaders(natsHeaders);

            NatsMsg<TReply> reply =
                await nats
                    .RequestAsync(
                        subject: Addressing.Subject<TService>(memberName),
                        data: request,
                        headers: natsHeaders,
                        requestSerializer: PolymorphicJsonSerializer.Create<TRequest>(),
                        replySerializer: PolymorphicJsonSerializer.Create<TReply>(),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            return reply.Data;
        }
    }
}
