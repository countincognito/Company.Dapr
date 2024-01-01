using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Company.iFX.Grpc
{
    public class TrackingContextGrpcInterceptor
        : Interceptor
    {
        /// <summary>
        /// Initializes a new instance of the TrackingContextCallInvoker class.
        /// </summary>
        /// <param name="channel">Channel to use.</param>
        public TrackingContextGrpcInterceptor()
        {
        }

        /// <summary>
        /// Intercept and add headers to a BlockingUnaryCall.
        /// </summary>
        /// <param name="request">The request to intercept.</param>
        /// <param name="context">The client interceptor context to add headers to.</param>
        /// <param name="continuation">The continuation of the request after all headers have been added.</param>
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            AddCallerMetadata(ref context);

            return continuation(request, context);
        }

        /// <summary>
        /// Intercept and add headers to a AsyncUnaryCall.
        /// </summary>
        /// <param name="request">The request to intercept.</param>
        /// <param name="context">The client interceptor context to add headers to.</param>
        /// <param name="continuation">The continuation of the request after all headers have been added.</param>
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            AddCallerMetadata(ref context);

            return continuation(request, context);
        }

        /// <summary>
        /// Intercept and add headers to a AsyncClientStreamingCall.
        /// </summary>
        /// <param name="context">The client interceptor context to add headers to.</param>
        /// <param name="continuation">The continuation of the request after all headers have been added.</param>
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            AddCallerMetadata(ref context);

            return continuation(context);
        }

        /// <summary>
        /// Intercept and add headers to a AsyncServerStreamingCall.
        /// </summary>
        /// <param name="request">The request to intercept.</param>
        /// <param name="context">The client interceptor context to add headers to.</param>
        /// <param name="continuation">The continuation of the request after all headers have been added.</param>
        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            AddCallerMetadata(ref context);

            return continuation(request, context);
        }

        /// <summary>
        /// Intercept and add headers to a AsyncDuplexStreamingCall.
        /// </summary>
        /// <param name="context">The client interceptor context to add headers to.</param>
        /// <param name="continuation">The continuation of the request after all headers have been added.</param>
        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            AddCallerMetadata(ref context);

            return continuation(context);
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
              TRequest request,
              ServerCallContext context,
              UnaryServerMethod<TRequest, TResponse> continuation)
        {
            AddCallerMetadata(ref context);

            return await continuation(request, context).ConfigureAwait(false);
        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            ServerCallContext context,
            ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            AddCallerMetadata(ref context);

            return await continuation(requestStream, context).ConfigureAwait(false);
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
            TRequest request,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            AddCallerMetadata(ref context);

            await continuation(request, responseStream, context).ConfigureAwait(false);
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            AddCallerMetadata(ref context);

            await continuation(requestStream, responseStream, context).ConfigureAwait(false);
        }

        private static void AddCallerMetadata<TRequest, TResponse>(ref ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            Metadata headers = context.Options.Headers ?? new Metadata();
            CallOptions updatedOptions = context.Options.WithHeaders(TrackingContextHelper.ProcessHeaders(headers));
            context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, updatedOptions);
        }

        private static void AddCallerMetadata(ref ServerCallContext context)
        {
            Metadata headers = context.RequestHeaders ?? new Metadata();
            Metadata updatedHeaders = TrackingContextHelper.ProcessHeaders(headers);
            context.WriteResponseHeadersAsync(updatedHeaders);
        }
    }
}
