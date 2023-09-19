using AutoMapper;
using Company.iFX.Proxy;
using Company.Utility.Encryption.Data;
using Company.Utility.Encryption.Interface;
using ProtoBuf.Grpc;
using Serilog;
using Zametek.Utility.Logging;

namespace Company.Utility.Encryption.Impl
{
    [DiagnosticLogging(LogActive.On)]
    public class EncryptionUtility
        : IEncryptionUtility
    {
        private readonly ILogger m_Logger;
        private readonly Zametek.Utility.Encryption.IEncryptionUtility m_EncryptionUtility;
        private readonly IMapper m_Mapper;

        public EncryptionUtility()
        {
            m_Logger = Proxy.CreateLogger<IEncryptionUtility>();
            m_EncryptionUtility = Proxy.Create<Zametek.Utility.Encryption.IEncryptionUtility>(m_Logger);
            m_Mapper = iFX.Container.Container.GetService<IMapper>();
        }

        public async Task<CreateKeysResponse> CreateKeysAsync(
            CreateKeysRequest request,
            CallContext context = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            m_Logger.Information($"{nameof(CreateKeysAsync)} Invoked");

            Zametek.Utility.Encryption.CreateKeysResponse response = await m_EncryptionUtility.CreateKeysAsync(
                m_Mapper.Map<Zametek.Utility.Encryption.CreateKeysRequest>(request),
                context.CancellationToken);

            return m_Mapper.Map<CreateKeysResponse>(response);
        }

        public async Task<EncryptResponse> EncryptAsync(
            EncryptRequest request,
            CallContext context = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            m_Logger.Information($"{nameof(EncryptAsync)} Invoked");

            Zametek.Utility.Encryption.EncryptResponse response = await m_EncryptionUtility.EncryptAsync(
                m_Mapper.Map<Zametek.Utility.Encryption.EncryptRequest>(request),
                context.CancellationToken);

            return m_Mapper.Map<EncryptResponse>(response);
        }

        public async Task<DecryptResponse> DecryptAsync(
            DecryptRequest request,
            CallContext context = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            m_Logger.Information($"{nameof(DecryptAsync)} Invoked");

            Zametek.Utility.Encryption.DecryptResponse response = await m_EncryptionUtility.DecryptAsync(
                m_Mapper.Map<Zametek.Utility.Encryption.DecryptRequest>(request),
                context.CancellationToken);

            return m_Mapper.Map<DecryptResponse>(response);
        }

        public async Task<RotateAsymmetricKeyResponse> RotateAsymmetricKeyAsync(
            RotateAsymmetricKeyRequest request,
            CallContext context = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            m_Logger.Information($"{nameof(RotateAsymmetricKeyAsync)} Invoked");

            Zametek.Utility.Encryption.RotateAsymmetricKeyResponse response = await m_EncryptionUtility.RotateAsymmetricKeyAsync(
                m_Mapper.Map<Zametek.Utility.Encryption.RotateAsymmetricKeyRequest>(request),
                context.CancellationToken);

            return m_Mapper.Map<RotateAsymmetricKeyResponse>(response);
        }

        public async Task<ViewSymmetricKeyDefinitionResponse> ViewSymmetricKeyDefinitionAsync(
            ViewSymmetricKeyDefinitionRequest request,
            CallContext context = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            m_Logger.Information($"{nameof(ViewSymmetricKeyDefinitionAsync)} Invoked");

            Zametek.Utility.Encryption.ViewSymmetricKeyDefinitionResponse response = await m_EncryptionUtility.ViewSymmetricKeyDefinitionAsync(
                m_Mapper.Map<Zametek.Utility.Encryption.ViewSymmetricKeyDefinitionRequest>(request),
                context.CancellationToken);

            return m_Mapper.Map<ViewSymmetricKeyDefinitionResponse>(response);
        }

        public async Task<ViewAsymmetricKeyDefinitionResponse> ViewAsymmetricKeyDefinitionAsync(
            ViewAsymmetricKeyDefinitionRequest request,
            CallContext context = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            m_Logger.Information($"{nameof(ViewAsymmetricKeyDefinitionAsync)} Invoked");

            Zametek.Utility.Encryption.ViewAsymmetricKeyDefinitionResponse response = await m_EncryptionUtility.ViewAsymmetricKeyDefinitionAsync(
                m_Mapper.Map<Zametek.Utility.Encryption.ViewAsymmetricKeyDefinitionRequest>(request),
                context.CancellationToken);

            return m_Mapper.Map<ViewAsymmetricKeyDefinitionResponse>(response);
        }
    }
}
