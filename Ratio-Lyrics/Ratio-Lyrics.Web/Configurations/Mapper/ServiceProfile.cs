using AutoMapper;
using Ratio_Lyrics.Web.Models;

namespace Ratio_Lyrics.Web.Configurations.Mapper
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {            
            CreateMap(typeof(PagedResponse<>), typeof(PagedResponse<>));
        }

    }
}
