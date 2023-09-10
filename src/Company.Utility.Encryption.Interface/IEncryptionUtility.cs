using Company.Utility.Encryption.Data;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace Company.Utility.Encryption.Interface
{
    [Service]
    public interface IEncryptionUtility
    {
        [Operation]
        Task<CreateKeysResponse> CreateKeysAsync(CreateKeysRequest request, CallContext context = default);

        [Operation]
        Task<EncryptResponse> EncryptAsync(EncryptRequest request, CallContext context = default);

        [Operation]
        Task<DecryptResponse> DecryptAsync(DecryptRequest request, CallContext context = default);

        [Operation]
        Task<RotateAsymmetricKeyResponse> RotateAsymmetricKeyAsync(RotateAsymmetricKeyRequest request, CallContext context = default);

        [Operation]
        Task<ViewSymmetricKeyDefinitionResponse> ViewSymmetricKeyDefinitionAsync(ViewSymmetricKeyDefinitionRequest request, CallContext context = default);

        [Operation]
        Task<ViewAsymmetricKeyDefinitionResponse> ViewAsymmetricKeyDefinitionAsync(ViewAsymmetricKeyDefinitionRequest request, CallContext context = default);
    }
}