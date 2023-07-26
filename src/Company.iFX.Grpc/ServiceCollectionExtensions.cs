using Microsoft.Extensions.DependencyInjection;

namespace Company.iFX.Grpc
{
    public static class ServiceCollectionExtensions
    {
        #region Public Members

        public static IServiceCollection AddTrackingContextGrpcInterceptor(this IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.Interceptors.Add<TrackingContextGrpcInterceptor>();
            });
            return services;
        }

        #endregion
    }
}
