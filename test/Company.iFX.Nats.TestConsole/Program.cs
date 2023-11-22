using Company.Engine.Registration.Data;
using Company.Engine.Registration.Interface;

namespace Company.iFX.Nats.TestConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
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
                    RegisterResponseBase engineMobileResponse = await engine.RegisterAsync(engineMobileRequest);
                    Console.WriteLine(engineMobileResponse.Name);

                    RegisterResponseBase engineWebResponse = await engine.RegisterAsync(engineWebRequest);
                    Console.WriteLine(engineWebResponse.Name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex}: The request did not complete properly.");
                }

                Console.WriteLine("Press any key to send another message, or press 'q' to quit.");
                string? result = Console.ReadLine();

                if (result == "q")
                {
                    break;
                }
            }
        }
    }
}