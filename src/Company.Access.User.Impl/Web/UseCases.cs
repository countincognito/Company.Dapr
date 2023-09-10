using Company.Access.User.Data.Db;
using Company.Access.User.Data.Web;
using Company.Access.User.Interface.Web;
using Company.iFX.Proxy;
using Microsoft.EntityFrameworkCore;
using ProtoBuf.Grpc;
using Serilog;
using System.Diagnostics;
using Zametek.Utility;
using Zametek.Utility.Encryption;
using Zametek.Utility.Logging;

namespace Company.Access.User.Impl.Web
{
    [DiagnosticLogging(LogActive.On)]
    public class UseCases
        : IUseCases
    {
        private readonly ILogger m_Logger;
        private readonly IDbContextFactory<UserContext> m_CtxFactory;
        private readonly IEncryptionUtility m_EncryptionUtility;

        public UseCases()
        {
            m_Logger = Proxy.CreateLogger<IUseCases>();
            m_CtxFactory = iFX.Container.Container.GetService<IDbContextFactory<UserContext>>();
            m_EncryptionUtility = Proxy.Create<IEncryptionUtility>(m_Logger);
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

                NameValueSet? result = await ctx.NameValueSets
                    .Where(x => x.Name == registerRequest.Name)
                    .SingleOrDefaultAsync(context.CancellationToken);

                if (result is null)
                {
                    Debug.Assert(registerRequest is not null);
                    m_Logger.Warning(@"No DOB currently stored for name: {@Name}", registerRequest?.Name);

                    (SymmetricKeyDefinition symmetricKeyDefinition, AsymmetricKeyDefinition asymmetricKeyDefinition) =
                        await m_EncryptionUtility.CreateSymmetricKeyIdAsync(
                            registerRequest?.Name ?? Guid.NewGuid().ToDashedString(),
                            context.CancellationToken);

                    m_Logger.Information(@"Creating new Symmetric Key ID: {@ID}", symmetricKeyDefinition.Id);

                    EncryptionContext.NewCurrent(symmetricKeyDefinition.Id);

                    var input = new NameValueSet
                    {
                        Name = registerRequest?.Name ?? string.Empty,
                        Value = registerRequest?.DateOfBirth?.ToString("u") ?? string.Empty,
                        SymmetricKeyId = symmetricKeyDefinition.Id,
                        EncryptedValue = await m_EncryptionUtility.EncryptObjectAsync(registerRequest, context.CancellationToken)
                    };

                    using (var transaction = await ctx.Database.BeginTransactionAsync(context.CancellationToken))
                    {
                        try
                        {
                            await ctx.NameValueSets.AddAsync(input, context.CancellationToken);
                            await ctx.SaveChangesAsync(context.CancellationToken);
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            m_Logger.Error(ex, @"Failed to record a new NameValueSet in database for: {@RegisterRequest}.", registerRequest);
                            transaction.Rollback();
                        }
                    }

                    result = await ctx.NameValueSets
                        .Where(x => x.Name == input.Name)
                        .SingleOrDefaultAsync(context.CancellationToken);
                }
                else
                {
                    EncryptionContext.NewCurrent(result!.SymmetricKeyId);

                    m_Logger.Warning(@"DOB already stored for name: {@Name}", registerRequest.Name);
                }

                RegisterRequest output = await m_EncryptionUtility
                    .DecryptObjectAsync<RegisterRequest>(result!.EncryptedValue, context.CancellationToken);

                webMessage = output?.DateOfBirth?.ToString("u") ?? @"No DOB";
            }
            catch (Exception ex)
            {
                webMessage = "Something weird happened!";
                m_Logger.Error(ex, @"Unable to store/encrypt DOB for name: {@Name}", registerRequest.Name);
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
