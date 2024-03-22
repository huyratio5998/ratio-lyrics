using Ratio_Lyrics.Web.Models;

namespace Ratio_Lyrics.Web.Services.Abstraction
{
    public interface ISongService
    {
        public Task<int> CreateSongAsync(SongViewModel newSong);        
        Task<SongViewModel?> GetSongAsync(string text, bool isTracking = true);
        public Task<SongViewModel?> GetSongAsync(int songId, bool isTracking = true);
        Task<PagedResponse<SongViewModel>> GetSuggestSongsAsync(BaseQueryParams queryParams);
        public Task<PagedResponse<SongViewModel>> GetSongsAsync(BaseQueryParams queryParams);
        public Task<bool> UpdateSongAsync(SongViewModel newSong);
        public Task<SongViewsResponseViewModel> UpdateViewsAsync(int songId, CancellationToken token);
        public Task<bool> DeleteSongAsync(int id);

        public string BuildSearchKey(SongViewModel newSong);
    }
}
