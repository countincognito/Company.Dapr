using Company.Engine.Registration.Data;
using Company.Engine.Registration.Interface;
using Zametek.Utility;

namespace Company.iFX.Nats.TestConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to send another message, or press 'q' to quit.");
                string? result = Console.ReadLine();

                if (result == "q")
                {
                    break;
                }

                IRegistrationEngine engine = new RegistrationEngineNatsProxy();

                RegisterRequestBase engineMobileRequest = new Engine.Registration.Data.Mobile.RegisterRequest
                {
                    Name = @$"Mobile engine request sent at: {DateTimeOffset.Now}",
                    Email = "email",
                    Password = "password",
                };

                RegisterRequestBase engineWebRequest = new Engine.Registration.Data.Web.RegisterRequest
                {
                    Name = @$"Web engine request sent at: {DateTimeOffset.Now}",
                    Email = "email",
                    DateOfBirth = DateTime.Now,
                };

                try
                {
                    TrackingContext.NewCurrent();

                    Console.WriteLine($@"CallChainId = {TrackingContext.Current.CallChainId}");

                    var engineMobileResponse = await engine
                        .RegisterAsync(engineMobileRequest)
                        .ConfigureAwait(false) as Engine.Registration.Data.Mobile.RegisterResponse;
                    Console.WriteLine(engineMobileResponse!.Name);
                    Console.WriteLine(engineMobileResponse!.MobileMessage);

                    var engineWebResponse = await engine
                        .RegisterAsync(engineWebRequest)
                        .ConfigureAwait(false) as Engine.Registration.Data.Web.RegisterResponse;
                    Console.WriteLine(engineWebResponse!.Name);
                    Console.WriteLine(engineWebResponse!.WebMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex}: The request did not complete properly.");
                }
            }
        }
    }
}