using Company.Access.User.Data.Db;
using Company.Access.User.Data.Web;
using Company.Access.User.Interface.Web;
using Company.iFX.Proxy;
using Microsoft.EntityFrameworkCore;
using ProtoBuf.Grpc;
using Serilog;
using System.Diagnostics;
using Zametek.Utility;
using Zametek.Utility.Logging;

namespace Company.Access.User.Impl.Web
{
    [DiagnosticLogging(LogActive.On)]
    public class UseCases
        : IUseCases
    {
        private readonly ILogger m_Logger;
        private readonly IDbContextFactory<UserContext> m_CtxFactory;

        public UseCases()
        {
            m_Logger = Proxy.CreateLogger<IUseCases>();
            m_CtxFactory = iFX.Container.Container.GetService<IDbContextFactory<UserContext>>();

            using var ctx = m_CtxFactory.CreateDbContext();
            ctx.Database.Migrate();
        }

        public async Task<RegisterResponse> RegisterAsync(
            RegisterRequest registerRequest,
            CallContext context = default)
        {
            m_Logger.Information($"{nameof(RegisterAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterAsync)} {registerRequest.Name}");

            string webMessage = string.Empty;

            try
            {
                using var ctx = await m_CtxFactory.CreateDbContextAsync(context.CancellationToken);

                NameValuePair? result = await ctx.NameValuePairs
                    .Where(x => x.Name == registerRequest.Name)
                    .SingleOrDefaultAsync(context.CancellationToken);

                if (result is null)
                {
                    Debug.Assert(registerRequest is not null);
                    m_Logger.Warning(@"No DOB currently stored for name: {@Name}", registerRequest?.Name);

                    var input = new NameValuePair
                    {
                        Name = registerRequest?.Name ?? string.Empty,
                        Value = registerRequest.ObjectToByteArray(),
                    };

                    using (var transaction = await ctx.Database.BeginTransactionAsync(context.CancellationToken))
                    {
                        try
                        {
                            await ctx.NameValuePairs.AddAsync(input, context.CancellationToken);
                            await ctx.SaveChangesAsync(context.CancellationToken);
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            m_Logger.Error(ex, @"Failed to record a new NameValuePair in database for: {@RegisterRequest}.", registerRequest);
                            transaction.Rollback();
                        }
                    }

                    result = await ctx.NameValuePairs
                        .Where(x => x.Name == input.Name)
                        .SingleOrDefaultAsync(context.CancellationToken);
                }
                else
                {
                    m_Logger.Warning(@"DOB already stored for name: {@Name}", registerRequest.Name);
                }

                webMessage = result?.Value.ByteArrayToObject<RegisterRequest>()?.DateOfBirth.ToString() ?? @"No DOB";
            }
            catch (Exception ex)
            {
                webMessage = "Something weird happened!";
                m_Logger.Error(ex, @"Unable to cache DOB for name: {@Name}", registerRequest.Name);
            }

            RegisterResponse response = new()
            {
                Name = registerRequest?.Name,
                WebMessage = webMessage,
            };

            return response;
        }
    }
}
