using Grpc.Core;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Zametek.Utility;

namespace Company.iFX.Grpc
{
    public partial class TrackingContextHelper
    {
        [GeneratedRegex(@"[^a-z0-9_-]")]
        private static partial Regex InvalidHeaderKeyCharacterRegex();
        private static readonly string s_TrackingContextKeyName;

        static TrackingContextHelper()
        {
            s_TrackingContextKeyName = string.Concat(
                InvalidHeaderKeyCharacterRegex().Replace(TrackingContext.FullName.ToLowerInvariant(), "-"),
                Metadata.BinaryHeaderSuffix);
        }

        public static Metadata ProcessHeaders(Metadata headers)
        {
            if (headers is null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            Metadata.Entry? trackingEntry = headers.FirstOrDefault(x => string.CompareOrdinal(x.Key, s_TrackingContextKeyName) == 0);

            // Retrieve the tracking context from the message header, if it exists.
            if (trackingEntry is not null)
            {
                // If an tracking context exists in the message header, always use it to replace the ambient context.
                TrackingContext tc = TrackingContext.DeSerialize(trackingEntry.ValueBytes);
                tc.SetAsCurrent();
            }
            else
            {
                // If no tracking context exists then create one.
                TrackingContext.NewCurrentIfEmpty();

                Debug.Assert(TrackingContext.Current is not null);

                // Copy the tracking context to the message header.
                byte[] byteArray = TrackingContext.Serialize(TrackingContext.Current);
                headers.Add(s_TrackingContextKeyName, byteArray);
            }

            return headers;
        }
    }
}
