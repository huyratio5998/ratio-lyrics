using AutoMapper;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Models;

namespace Ratio_Lyrics.Web.Configurations.Mapper
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<Artist, ArtistViewModel>();
            CreateMap<ArtistViewModel, Artist>();

            CreateMap<MediaPlatform, MediaPlatformViewModel>();
            CreateMap<MediaPlatformViewModel, MediaPlatform>();

            CreateMap<Song, SongViewModel>()
                .ForMember(x=>x.Lyric, y=>y.MapFrom(t=>t.Lyric.Lyric))
                .ForMember(x=>x.Views, y=>y.MapFrom(t=>t.Lyric.Views));            
            CreateMap<SongViewModel, Song>();
            CreateMap<ArtistViewModel, SongArtist>()
                .ForMember(x=>x.ArtistId, y=>y.MapFrom(t=>t.Id));
            CreateMap<SongArtist, ArtistViewModel>()
                .ForMember(x=>x.Id, y=>y.MapFrom(t=>t.ArtistId));
            CreateMap<SongMediaPlatform, SongMediaPlatformViewModel>();
            CreateMap<SongMediaPlatformViewModel, SongMediaPlatform>();                

            CreateMap(typeof(PagedResponse<>), typeof(PagedResponse<>));
        }

    }
}
