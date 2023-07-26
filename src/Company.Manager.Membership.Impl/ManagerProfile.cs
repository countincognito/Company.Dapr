using AutoMapper;

namespace Company.Manager.Membership.Impl
{
    public class ManagerProfile
        : Profile
    {
        public ManagerProfile()
        {
            CreateMap<Data.Mobile.RegisterRequest, Engine.Registration.Data.Mobile.RegisterRequest>();
            CreateMap<Data.Web.RegisterRequest, Engine.Registration.Data.Web.RegisterRequest>();

            CreateMap<Data.RegisterRequestBase, Engine.Registration.Data.RegisterRequestBase>()
                .Include<Data.Mobile.RegisterRequest, Engine.Registration.Data.Mobile.RegisterRequest>()
                .Include<Data.Web.RegisterRequest, Engine.Registration.Data.Web.RegisterRequest>();

            CreateMap<Engine.Registration.Data.Mobile.RegisterResponse, Data.Mobile.RegisterResponse>();
            CreateMap<Engine.Registration.Data.Web.RegisterResponse, Data.Web.RegisterResponse>();

            CreateMap<Engine.Registration.Data.RegisterResponseBase, Data.RegisterResponseBase>()
                .Include<Engine.Registration.Data.Mobile.RegisterResponse, Data.Mobile.RegisterResponse>()
                .Include<Engine.Registration.Data.Web.RegisterResponse, Data.Web.RegisterResponse>();
        }
    }
}
