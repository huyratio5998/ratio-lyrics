using Ratio_Lyrics.Web.Models;

namespace Ratio_Lyrics.Web.Services.Abstraction
{
    public interface ISongService
    {
        public Task<int> CreateSongAsync(SongViewModel newSong);
        Task<SongViewModel?> GetSongAsync(string text);
        public Task<SongViewModel?> GetSongAsync(int songId, bool isTracking = true);
        public Task<PagedResponse<SongViewModel>> GetSongsAsync(BaseQueryParams queryParams);
        public Task<bool> UpdateSongAsync(SongViewModel newSong);
    }
}
