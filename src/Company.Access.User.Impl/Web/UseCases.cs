using Company.Access.User.Data.Db;
using Company.Access.User.Data.Web;
using Company.Access.User.Interface.Web;
using Company.iFX.Proxy;
using Company.Utility.Encryption.Data;
using Company.Utility.Encryption.Interface;
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

                var encryptionUtility = Proxy.Create<IEncryptionUtility>();

                if (result is null)
                {
                    Debug.Assert(registerRequest is not null);
                    m_Logger.Warning(@"No DOB currently stored for name: {@Name}", registerRequest?.Name);

                    m_Logger.Information(@"Encrypting data");

                    var createKeysRequest = new CreateKeysRequest
                    {
                        SymmetricKeyName = registerRequest?.Name ?? Guid.NewGuid().ToDashedString(),
                        AsymmetricKeyName = Guid.NewGuid().ToDashedString(),
                    };

                    CreateKeysResponse createKeyResponse = await encryptionUtility
                        .CreateKeysAsync(createKeysRequest, context.CancellationToken);

                    SymmetricKeyDefinition symmetricKeyDefinition = createKeyResponse.SymmetricKeyDefinition!;

                    m_Logger.Information(@"Creating new Symmetric Key ID: {@ID}", symmetricKeyDefinition.Id);

                    var encryptRequest = new EncryptRequest
                    {
                        SymmetricKeyId = symmetricKeyDefinition.Id,
                        Data = registerRequest.ObjectToByteArray(),
                    };

                    EncryptResponse encryptResponse = await encryptionUtility
                        .EncryptAsync(encryptRequest, context.CancellationToken);

                    var input = new NameValueSet
                    {
                        Name = registerRequest?.Name ?? string.Empty,
                        Value = registerRequest?.DateOfBirth?.ToString("u") ?? string.Empty,
                        SymmetricKeyId = symmetricKeyDefinition.Id,
                        EncryptedValue = encryptResponse.EncryptedData,
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
                    m_Logger.Warning(@"DOB already stored for name: {@Name}", registerRequest.Name);
                }

                var decryptRequest = new DecryptRequest
                {
                    SymmetricKeyId = result!.SymmetricKeyId,
                    EncryptedData = result!.EncryptedValue ?? Array.Empty<byte>(),
                };

                DecryptResponse decryptResponse = await encryptionUtility
                    .DecryptAsync(decryptRequest, context.CancellationToken);

                RegisterRequest output = decryptResponse.Data.ByteArrayToObject<RegisterRequest>();

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
