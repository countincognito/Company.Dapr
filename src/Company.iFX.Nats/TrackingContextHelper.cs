using Microsoft.Extensions.Primitives;
using NATS.Client.Core;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Zametek.Utility;

namespace Company.iFX.Nats
{
    public partial class TrackingContextHelper
    {
        [GeneratedRegex(@"[^a-z0-9_-]")]
        private static partial Regex InvalidHeaderKeyCharacterRegex();
        private static readonly string s_TrackingContextKeyName;

        static TrackingContextHelper()
        {
            s_TrackingContextKeyName = InvalidHeaderKeyCharacterRegex().Replace(TrackingContext.FullName.ToLowerInvariant(), "-");
        }

        public static NatsHeaders ProcessHeaders(NatsHeaders headers)
        {
            ArgumentNullException.ThrowIfNull(headers);

            // If the the tracking context exists in the header, retrieve it.
            if (headers.TryGetValue(s_TrackingContextKeyName, out StringValues values))
            {
                CheckHeaderValues(values);
            }
            else
            {
                // If no tracking context exists, then create one.
                byte[] byteArray = CreateAndSerializeNewTrackingContext();

                // Be sure to treat the StringValues as a singular string.
                headers.Add(s_TrackingContextKeyName, byteArray.ByteArrayToBase64String());
            }

            return headers;
        }

        private static void CheckHeaderValues(StringValues values)
        {
            // Be sure to treat the StringValues as a singular string.
            string tcBase64 = values.ToString();

            if (string.IsNullOrWhiteSpace(tcBase64))
            {
                // If no tracking context exists, then create one.
                TrackingContext.NewCurrentIfEmpty();
            }
            else
            {
                // If a tracking context exists in the header, always use it to replace the ambient context.
                TrackingContext tc = TrackingContext.DeSerialize(tcBase64.Base64StringToByteArray());
                tc.SetAsCurrent();
            }
        }

        private static byte[] CreateAndSerializeNewTrackingContext()
        {
            TrackingContext.NewCurrentIfEmpty();

            Debug.Assert(TrackingContext.Current is not null);

            // Copy the tracking context to the header.
            return TrackingContext.Serialize(TrackingContext.Current);
        }
    }
}
