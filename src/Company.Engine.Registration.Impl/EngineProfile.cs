using AutoMapper;

namespace Company.Engine.Registration.Impl
{
    public class EngineProfile
        : Profile
    {
        public EngineProfile()
        {
            CreateMap<Data.Mobile.RegisterRequest, Access.User.Data.Mobile.RegisterRequest>();
            CreateMap<Data.Web.RegisterRequest, Access.User.Data.Web.RegisterRequest>();

            CreateMap<Data.RegisterRequestBase, Access.User.Data.RegisterRequestBase>()
                .Include<Data.Mobile.RegisterRequest, Access.User.Data.Mobile.RegisterRequest>()
                .Include<Data.Web.RegisterRequest, Access.User.Data.Web.RegisterRequest>();

            CreateMap<Access.User.Data.Mobile.RegisterResponse, Data.Mobile.RegisterResponse>();
            CreateMap<Access.User.Data.Web.RegisterResponse, Data.Web.RegisterResponse>();

            CreateMap<Access.User.Data.RegisterResponseBase, Data.RegisterResponseBase>()
                .Include<Access.User.Data.Mobile.RegisterResponse, Data.Mobile.RegisterResponse>()
                .Include<Access.User.Data.Web.RegisterResponse, Data.Web.RegisterResponse>();
        }
    }
}
