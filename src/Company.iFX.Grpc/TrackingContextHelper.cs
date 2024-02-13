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
            ArgumentNullException.ThrowIfNull(headers);

            Metadata.Entry? trackingEntry = headers
                .FirstOrDefault(x => string.CompareOrdinal(x.Key, s_TrackingContextKeyName) == 0);

            // If the the tracking context exists in the header, retrieve it.
            if (trackingEntry is null)
            {
                // If no tracking context exists, then create one.
                TrackingContext.NewCurrentIfEmpty();

                Debug.Assert(TrackingContext.Current is not null);

                // Copy the tracking context to the header.
                byte[] byteArray = TrackingContext.Serialize(TrackingContext.Current);
                headers.Add(s_TrackingContextKeyName, byteArray);
            }
            else
            {
                // If a tracking context exists in the header, always use it to replace the ambient context.
                TrackingContext tc = TrackingContext.DeSerialize(trackingEntry.ValueBytes);
                tc.SetAsCurrent();
            }

            return headers;
        }
    }
}
