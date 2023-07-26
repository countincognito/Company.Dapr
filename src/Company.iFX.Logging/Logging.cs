using Destructurama;
using Serilog;
using Zametek.Utility.Logging;

namespace Company.iFX.Logging
{
    public static class Logging
    {
        public static LoggerConfiguration CreateConfiguration()
        {
            return new LoggerConfiguration()
                .Enrich.FromLogProxy()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Destructure.UsingAttributes();
        }
    }
}
