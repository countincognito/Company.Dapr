using Company.iFX.Common;
using Microsoft.Extensions.Primitives;
using NATS.Client.Core;
using System.Diagnostics;

namespace Company.iFX.Nats
{
    // https://github.com/nats-io/nats.net/discussions/720
    // https://stackoverflow.com/questions/73935038/how-to-pass-remote-parent-span-properly-using-nats/74477063#74477063
    public class OpenTracingHelper
    {
        public static NatsHeaders ProcessClientHeaders(NatsHeaders headers)
        {
            ArgumentNullException.ThrowIfNull(headers);

            // Attach telemetry headers.

            Activity? ac = Activity.Current;

            if (ac is not null)
            {
                headers.TryAdd(Constant.ActivityTraceIdName, ac.TraceId.ToString());
                headers.TryAdd(Constant.ActivitySpanIdName, ac.SpanId.ToString());
            }

            return headers;
        }

        public static ActivityContext GetParentContext(NatsHeaders headers)
        {
            ArgumentNullException.ThrowIfNull(headers);

            headers.TryGetValue(Constant.ActivityTraceIdName, out StringValues activityTraceId);
            headers.TryGetValue(Constant.ActivitySpanIdName, out StringValues activitySpanId);

            return new ActivityContext(
                traceId: ActivityTraceId.CreateFromString(activityTraceId.ToString()),
                spanId: ActivitySpanId.CreateFromString(activitySpanId.ToString()),
                ActivityTraceFlags.Recorded,
                isRemote: true);
        }
    }
}
