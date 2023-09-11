using AutoMapper;
using Company.Utility.Cache.Data;

namespace Company.Utility.Cache.Impl
{
    public class UtilityProfile
        : Profile
    {
        public UtilityProfile()
        {
            CreateMap<GetCachedValueRequest, Zametek.Utility.Cache.GetCachedValueRequest>();
            CreateMap<RefreshCachedValueRequest, Zametek.Utility.Cache.RefreshCachedValueRequest>();
            CreateMap<DeleteCachedValueRequest, Zametek.Utility.Cache.DeleteCachedValueRequest>();
            CreateMap<SetCachedValueRequest, Zametek.Utility.Cache.SetCachedValueRequest>();

            CreateMap<GetCachedValueResponse, Zametek.Utility.Cache.GetCachedValueResponse>()
                .ReverseMap();
        }
    }
}
