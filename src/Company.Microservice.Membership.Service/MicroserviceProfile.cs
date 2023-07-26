using AutoMapper;

namespace Company.Microservice.Membership.Service
{
    public class MicroserviceProfile
        : Profile
    {
        public MicroserviceProfile()
        {
            #region v1.0

            CreateMap<Data.v1_0.Mobile.RegisterRequestDto, Manager.Membership.Data.Mobile.RegisterRequest>();
            CreateMap<Data.v1_0.Web.RegisterRequestDto, Manager.Membership.Data.Web.RegisterRequest>();

            CreateMap<Data.v1_0.RegisterRequestDtoBase, Manager.Membership.Data.RegisterRequestBase>()
                .Include<Data.v1_0.Mobile.RegisterRequestDto, Manager.Membership.Data.Mobile.RegisterRequest>()
                .Include<Data.v1_0.Web.RegisterRequestDto, Manager.Membership.Data.Web.RegisterRequest>();


            CreateMap<Manager.Membership.Data.Mobile.RegisterResponse, Data.v1_0.Mobile.RegisterResponseDto>();
            CreateMap<Manager.Membership.Data.Web.RegisterResponse, Data.v1_0.Web.RegisterResponseDto>();


            CreateMap<Manager.Membership.Data.RegisterResponseBase, Data.v1_0.RegisterResponseDtoBase>()
                .Include<Manager.Membership.Data.Mobile.RegisterResponse, Data.v1_0.Mobile.RegisterResponseDto>()
                .Include<Manager.Membership.Data.Web.RegisterResponse, Data.v1_0.Web.RegisterResponseDto>();

            #endregion
        }
    }
}
