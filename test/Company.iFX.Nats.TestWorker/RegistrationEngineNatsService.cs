using Company.Engine.Registration.Data;
using Company.Engine.Registration.Interface;
using ProtoBuf.Grpc;
using Zametek.Utility;

namespace Company.iFX.Nats.TestWorker
{
    public class RegistrationEngineNatsService
        : NatsServiceBase, IRegistrationEngine
    {
        #region Private Members

        private static RegisterResponseBase? RegisterFunction(
            RegisterRequestBase? input,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(input);
            RegisterResponseBase? output = null;

            Console.WriteLine($"CallChainId = {TrackingContext.Current.CallChainId}");

            input.TypeSwitchOn()
                .Case<Engine.Registration.Data.Mobile.RegisterRequest>(x =>
                {
                    Console.WriteLine($"Received mobile request: {x.Name}");

                    output = new Engine.Registration.Data.Mobile.RegisterResponse
                    {
                        Name = @$"This is the Mobile response name: email = {x.Email}",
                        MobileMessage = $@"Mobile Message and CallChainId = {TrackingContext.Current.CallChainId}",
                    };
                })
                .Case<Engine.Registration.Data.Web.RegisterRequest>(x =>
                {
                    Console.WriteLine($"Received web request: {x.Name}");

                    output = new Engine.Registration.Data.Web.RegisterResponse
                    {
                        Name = @$"This is the Web response name: DOB = {x.DateOfBirth}",
                        WebMessage = $@"Web Message and CallChainId = {TrackingContext.Current.CallChainId}",
                    };
                })
                .Default(_ => throw new InvalidOperationException());

            Console.WriteLine();
            return output;
        }

        #endregion

        #region IRegistrationEngine Members

        public async Task<RegisterResponseBase> RegisterAsync(
            RegisterRequestBase request,
            CallContext context = default)
        {
            // No null checks here because the only value that will
            // ever be passed in is null.

            RegisterResponseBase? reply = await SubscribeAsync<IRegistrationEngine, RegisterRequestBase, RegisterResponseBase>(
                    RegisterFunction,
                    cancellationToken: context.CancellationToken)
                .ConfigureAwait(false);

            return reply!;
        }

        #endregion
    }
}
