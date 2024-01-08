using NATS.Client.Core;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
            Debug.Assert(typeof(TService).IsInterface);
            ArgumentNullException.ThrowIfNull(request);

            await using var nats = new NatsConnection(opts ?? NatsOpts.Default);

            // Add TrackingContext to headers.
            NatsHeaders natsHeaders = TrackingContextHelper.ProcessHeaders(headers ?? []);

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
