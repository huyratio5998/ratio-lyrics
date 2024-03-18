using AutoMapper;
using Ratio_Lyrics.Web.Areas.Admin.Models;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Models;

namespace Ratio_Lyrics.Web.Configurations.Mapper
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            //artist
            CreateMap<Artist, ArtistViewModel>();
            CreateMap<ArtistViewModel, Artist>();
            CreateMap<ArtistViewModel, SongArtist>()
                .ForMember(x => x.ArtistId, y => y.MapFrom(t => t.Id));
            CreateMap<SongArtist, ArtistViewModel>()
                .ForMember(x => x.Id, y => y.MapFrom(t => t.ArtistId))
                .ForMember(x => x.Name, y => y.MapFrom(t => t.Artist.Name))
                .ForMember(x => x.Description, y => y.MapFrom(t => t.Artist.Description))
                .ForMember(x => x.Role, y => y.MapFrom(t => t.Artist.Role));

            //media platform
            CreateMap<MediaPlatform, MediaPlatformViewModel>();
            CreateMap<MediaPlatformViewModel, MediaPlatform>();
            CreateMap<SongMediaPlatform, SongMediaPlatformViewModel>()
                .ForMember(x => x.MediaPlatformId, y => y.MapFrom(t => t.MediaPlatform.Id))
                .ForMember(x => x.Name, y => y.MapFrom(t => t.MediaPlatform.Name))
                .ForMember(x => x.Image, y => y.MapFrom(t => t.MediaPlatform.Image));
            CreateMap<SongMediaPlatformViewModel, SongMediaPlatform>();                
            CreateMap<MediaPlatformViewModel, SongMediaPlatformViewModel>()
                .ForMember(x => x.MediaPlatformId, y => y.MapFrom(t => t.Id));
            CreateMap<SongMediaPlatformViewModel, MediaPlatformViewModel>()
                .ForMember(x => x.Id, y => y.MapFrom(t => t.MediaPlatformId));

            //song
            CreateMap<Song, SongViewModel>()
                .ForMember(x=>x.Lyric, y=>y.MapFrom(t=>t.Lyric.Lyric))
                .ForMember(x=>x.Views, y=>y.MapFrom(t=>t.Lyric.Views))
                .ForMember(x=>x.Artists, y=>y.MapFrom(t=>t.SongArtists))                
                .ForMember(x => x.ImageUrl, y => y.MapFrom(t => t.Image))
                .ForMember(x => x.Image, y => y.Ignore()); ;            
            CreateMap<SongViewModel, Song>()
                .ForMember(x=>x.Image, y=>y.MapFrom(t=>t.ImageUrl))
                .ForMember(x=>x.Lyric, y=>y.Ignore())
                .ForMember(x => x.SongArtists, y => y.Ignore())
                .ForMember(x => x.MediaPlatformLinks, y => y.Ignore());

            CreateMap(typeof(PagedResponse<>), typeof(PagedResponse<>));

            //admin
            CreateMap<PagedResponse<SongViewModel>, ListSongsAdminViewModel>();

        }

    }
}
