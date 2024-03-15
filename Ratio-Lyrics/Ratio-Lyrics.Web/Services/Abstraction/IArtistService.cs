using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Models;

namespace Ratio_Lyrics.Web.Services.Abstraction
{
    public interface IArtistService
    {
        Task<ArtistViewModel?> GetArtist(int ArtistId, bool isTracking = true);
        Task<PagedResponse<ArtistViewModel>> GetArtistsAsync(BaseQueryParams query);
        Task<Artist?> CreateArtistRequestAsync(ArtistViewModel newArtist);
        Task<int> CreateArtistAsync(ArtistViewModel newArtist);
        Task<bool> DeleteArtistAsync(int ArtistId);
        Task<bool> UpdateArtistAsync(ArtistViewModel newArtist);
    }
}
