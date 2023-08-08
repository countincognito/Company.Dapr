using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Company.iFX.Api
{
    public static class Handlers
    {
        public static void ExceptionHandler(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                IExceptionHandlerPathFeature? exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                Exception? exception = exceptionHandlerPathFeature?.Error;
                if (exception?.InnerException is AggregateException
                    && exception.InnerException?.InnerException is HttpRequestException)
                {
                    await context.Response.WriteAsync("Network or server error calling down stream service");
                };
            });
        }
    }
}
