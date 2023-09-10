using AutoMapper;
using Company.Utility.Encryption.Data;

namespace Company.Utility.Encryption.Impl
{
    public class UtilityProfile
        : Profile
    {
        public UtilityProfile()
        {
            CreateMap<CreateKeysRequest, Zametek.Utility.Encryption.CreateKeysRequest>()
                .ReverseMap();
            CreateMap<EncryptRequest, Zametek.Utility.Encryption.EncryptRequest>()
                .ReverseMap();
            CreateMap<DecryptRequest, Zametek.Utility.Encryption.DecryptRequest>()
                .ReverseMap();
            CreateMap<RotateAsymmetricKeyRequest, Zametek.Utility.Encryption.RotateAsymmetricKeyRequest>()
                .ReverseMap();
            CreateMap<ViewSymmetricKeyDefinitionRequest, Zametek.Utility.Encryption.ViewSymmetricKeyDefinitionRequest>()
                .ReverseMap();
            CreateMap<ViewAsymmetricKeyDefinitionRequest, Zametek.Utility.Encryption.ViewAsymmetricKeyDefinitionRequest>()
                .ReverseMap();

            CreateMap<CreateKeysResponse, Zametek.Utility.Encryption.CreateKeysResponse>()
                .ReverseMap();
            CreateMap<EncryptResponse, Zametek.Utility.Encryption.EncryptResponse>()
                .ReverseMap();
            CreateMap<DecryptResponse, Zametek.Utility.Encryption.DecryptResponse>()
                .ReverseMap();
            CreateMap<RotateAsymmetricKeyResponse, Zametek.Utility.Encryption.RotateAsymmetricKeyResponse>()
                .ReverseMap();
            CreateMap<ViewSymmetricKeyDefinitionResponse, Zametek.Utility.Encryption.ViewSymmetricKeyDefinitionResponse>()
                .ReverseMap();
            CreateMap<ViewAsymmetricKeyDefinitionResponse, Zametek.Utility.Encryption.ViewAsymmetricKeyDefinitionResponse>()
                .ReverseMap();

            CreateMap<AsymmetricKeyDefinition, Zametek.Utility.Encryption.AsymmetricKeyDefinition>()
                .ReverseMap();
            CreateMap<SymmetricKeyDefinition, Zametek.Utility.Encryption.SymmetricKeyDefinition>()
                .ReverseMap();
        }
    }
}
