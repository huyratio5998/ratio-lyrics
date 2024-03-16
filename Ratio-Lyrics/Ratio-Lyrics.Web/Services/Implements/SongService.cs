using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Ratio_Lyrics.Web.Constants;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Helpers;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Repositories.Abstracts;
using Ratio_Lyrics.Web.Services.Abstraction;
using System.Text.Json;

namespace Ratio_Lyrics.Web.Services.Implements
{
    public class SongService : ISongService
    {
        private readonly IArtistService _artistService;
        private readonly IMediaPlatformService _mediaPlatformService;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IValidator<SongViewModel> _validator;
        private readonly ILogger _logger;
        private readonly IBaseRepository<Song> _songRepository;
        private const string wwwRootAddress = "wwwroot";

        public SongService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<SongViewModel> validator,
            IWebHostEnvironment webHostEnvironment,
            IArtistService artistService,
            IMediaPlatformService mediaPlatformService,
            ILogger<SongService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
            _webHostEnvironment = webHostEnvironment;
            _artistService = artistService;
            _mediaPlatformService = mediaPlatformService;
            _songRepository = _unitOfWork.GetRepository<Song>();
            _logger = logger;
        }

        public async Task<int> CreateSongAsync(SongViewModel newSong)
        {
            try
            {
                _logger.LogInformation("Create song in-process:");
                var validateResult = await _validator.ValidateAsync(newSong);
                if (!validateResult.IsValid)
                {
                    _logger.LogError($"Invalid request: {JsonSerializer.Serialize(newSong)}");
                    return 0;
                }

                await _unitOfWork.CreateTransactionAsync();
                Task uploadImage = null;
                if (newSong.Image != null)
                {
                    _logger.LogInformation("Start saving song image");
                    var webRootAddress = string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath) ? wwwRootAddress : _webHostEnvironment.WebRootPath;
                    var saveFileName = FileHelpers.FileNameFormatDateTime(newSong.Image.FileName);
                    uploadImage = FileHelpers.UploadFile(newSong.Image, saveFileName, webRootAddress, FileConstants.ImageFolder, FileConstants.SongFolder);
                    newSong.ImageUrl = FileHelpers.ResolveImage(FileConstants.SongImageBaseUrl, saveFileName);
                }

                _logger.LogInformation("Start create song");
                var song = _mapper.Map<Song>(newSong);
                song.Id = 0;
                var createdSong = await _songRepository.CreateAsync(song);
                await _unitOfWork.SaveAsync();
                newSong.Id = createdSong.Id;
                if (createdSong == null || createdSong.Id == 0) return 0;
                _logger.LogInformation($"Create song: {newSong.Id}");

                //create addition info
                await AddSongLyric(newSong);
                await AddSongArtist(newSong);
                await AddMediaPlatformLinks(newSong);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"Create addition song info");

                if (uploadImage != null) await uploadImage;
                await _unitOfWork.CommitAsync();
                _logger.LogInformation($"Commit create song successfully");

                return createdSong.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception to create song: {ex}");
                _logger.LogError($"Request object make fail: {JsonSerializer.Serialize(newSong)}");
                await _unitOfWork.RollbackAsync();
                return 0;
            }
        }

        private async Task AddSongLyric(SongViewModel newSong)
        {
            await _unitOfWork.GetRepository<SongLyric>().CreateAsync(new SongLyric
            {
                Lyric = newSong.Lyric,
                Views = 0,
                SongId = newSong.Id
            });
            _logger.LogInformation($"Save song lyrics");
        }

        private async Task AddSongArtist(SongViewModel song)
        {
            if (song == null || song.Artists == null || !song.Artists.Any()) return;
            var songArtistRepository = _unitOfWork.GetRepository<SongArtist>();

            foreach (var artist in song.Artists)
            {
                if (artist == null) continue;

                var artistExisted = await _artistService.GetArtist(artist?.Id ?? 0, false);
                var createdArtistId = artistExisted?.Id ?? 0;

                if (createdArtistId == 0)
                    createdArtistId = await _artistService.CreateArtistAsync(artist);

                await songArtistRepository
                    .CreateAsync(new SongArtist
                    {
                        SongId = song.Id,
                        ArtistId = createdArtistId
                    });
            }
            _logger.LogInformation($"Save song artist");
        }

        private async Task AddMediaPlatformLinks(SongViewModel song)
        {
            if (song == null || song.MediaPlatformLinks == null || !song.MediaPlatformLinks.Any()) return;
            var songMediaPlatformRepository = _unitOfWork.GetRepository<SongMediaPlatform>();

            foreach (var mediaPlatform in song.MediaPlatformLinks)
            {
                if (mediaPlatform == null || mediaPlatform.MediaPlatformId == 0) continue;

                var media = await _mediaPlatformService.GetMediaPlatform(mediaPlatform.MediaPlatformId, false);
                if (media == null) continue;

                await songMediaPlatformRepository.CreateAsync(new SongMediaPlatform
                {
                    Link = mediaPlatform.Link,
                    SongId = song.Id,
                    MediaPlatformId = media.Id
                });
            }
            _logger.LogInformation($"Save song media links");
        }

        public async Task<SongViewModel?> GetSongAsync(int songId, bool isTracking = true)
        {
            if (songId <= 0) return new SongViewModel();

            var song = await _songRepository
                .GetAll(isTracking).AsQueryable()
                .Include(x => x.Lyric)
                .Include(x => x.MediaPlatformLinks)
                .ThenInclude(song => song.MediaPlatform)
                .Include(x => x.SongArtists)
                .ThenInclude(song =>song.Artist)
                .FirstOrDefaultAsync(x => x.Id == songId);

            if (song == null || song.Id == 0) return null;

            return _mapper.Map<SongViewModel>(song);
        }
        public async Task<SongViewModel?> GetSongAsync(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            var song = await _songRepository
                .GetAll().AsQueryable()
                .Include(x => x.Lyric)
                .Include(x => x.MediaPlatformLinks)
                .Include(x => x.SongArtists)
                .FirstOrDefaultAsync(x => x.Name.Contains(text)
                    || x.DisplayName.Contains(text)
                    || x.SearchKey.Contains(text));

            if (song == null || song.Id == 0) return null;

            return _mapper.Map<SongViewModel>(song);
        }

        public async Task<PagedResponse<SongViewModel>> GetSongsAsync(BaseQueryParams queryParams)
        {
            IQueryable<Song> items = _songRepository
                            .GetAll().AsQueryable()
                            .Include(x => x.Lyric)
                            .Include(x => x.MediaPlatformLinks)
                            .Include(x => x.SongArtists);

            // filter
            if (!string.IsNullOrWhiteSpace(queryParams.SearchText))
                items = items
                    .Where(x => x.Name.Contains(queryParams.SearchText)
                    || x.DisplayName.Contains(queryParams.SearchText)
                    || x.SearchKey.Contains(queryParams.SearchText));

            // order
            var orderCondition = queryParams.OrderBy;
            if (orderCondition != null)
            {
                if (orderCondition == OrderType.Asc) items = items.OrderBy(x => x.Id);
                else if (orderCondition == OrderType.Desc) items = items.OrderByDescending(y => y.Id);
            }

            // paging
            queryParams.PageNumber = queryParams.PageNumber <= 0
                ? Models.CommonConstant.PageIndexDefault : queryParams.PageNumber;
            queryParams.PageSize = queryParams.PageSize <= 0
                ? Models.CommonConstant.PageSizeDefault : queryParams.PageSize;

            var songs = await PagedResponse<Song>
                .CreateAsync(items, queryParams.PageNumber, queryParams.PageSize);

            var result = _mapper.Map<PagedResponse<SongViewModel>>(songs);
            return result;
        }

        public async Task<bool> UpdateSongAsync(SongViewModel newSong)
        {
            try
            {
                _logger.LogInformation("Update song in-process:");
                var validateResult = await _validator.ValidateAsync(newSong);
                if (!validateResult.IsValid || newSong.Id == 0)
                {
                    _logger.LogError($"Invalid request: {JsonSerializer.Serialize(newSong)}");
                    return false;
                }

                var targetSong = await GetSongAsync(newSong.Id, false);
                if (targetSong == null)
                {
                    _logger.LogError($"Song not exist: {JsonSerializer.Serialize(newSong)}");
                    return false;
                }

                await _unitOfWork.CreateTransactionAsync();
                Task uploadImage = null;
                if (newSong.Image != null)
                {
                    _logger.LogInformation("Start saving song image");
                    var webRootAddress = string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath) ? wwwRootAddress : _webHostEnvironment.WebRootPath;
                    var saveFileName = FileHelpers.FileNameFormatDateTime(newSong.Image.FileName);
                    uploadImage = FileHelpers.UploadFile(newSong.Image, saveFileName, webRootAddress, FileConstants.ImageFolder, FileConstants.SongFolder);
                    newSong.ImageUrl = FileHelpers.ResolveImage(FileConstants.SongImageBaseUrl, saveFileName);
                }

                _logger.LogInformation("Start update song");
                var res = _songRepository.Update(_mapper.Map<Song>(newSong));
                if (!res)
                {
                    _logger.LogError($"Update song fail: {JsonSerializer.Serialize(newSong)}");
                    return false;
                }

                await UpdateSongLyric(newSong);
                await UpdateSongArtist(newSong);
                await UpdateMediaFlatform(newSong);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"Update addition song info");

                if (uploadImage != null) await uploadImage;
                await _unitOfWork.CommitAsync();
                _logger.LogInformation($"Commit update song successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception to update song: {ex}");
                _logger.LogError($"Request object make fail: {JsonSerializer.Serialize(newSong)}");
                await _unitOfWork.RollbackAsync();
                return false;
            }
        }
        private async Task UpdateSongLyric(SongViewModel newSong)
        {
            var currentLyrics = await _unitOfWork.GetRepository<SongLyric>()
                .GetAll(true).AsQueryable()
                .FirstOrDefaultAsync(x => x.SongId == newSong.Id);

            if (currentLyrics == null) return;

            currentLyrics.Lyric = newSong.Lyric;
            _logger.LogInformation($"Update song lyrics");
        }

        private async Task UpdateSongArtist(SongViewModel newSong)
        {
            var songArtistRepository = _unitOfWork.GetRepository<SongArtist>();
            var songArtist = songArtistRepository
                .GetAll().AsQueryable()
                .Where(x => x.SongId == newSong.Id)
                .ToList();

            if (songArtist != null && songArtist.Any())
                await songArtistRepository.DeleteRangeAsync(songArtist);

            await AddSongArtist(newSong);
            _logger.LogInformation($"Update song artist");
        }

        private async Task UpdateMediaFlatform(SongViewModel newSong)
        {
            var songMediaFlatformRepository = _unitOfWork.GetRepository<SongMediaPlatform>();
            var mediaLinks = songMediaFlatformRepository
                .GetAll().AsQueryable()
                .Where(x => x.SongId == newSong.Id)
                .ToList();

            if (mediaLinks != null && mediaLinks.Any())
                await songMediaFlatformRepository.DeleteRangeAsync(mediaLinks);

            await AddMediaPlatformLinks(newSong);
            _logger.LogInformation($"Update media platform link");
        }

        public async Task<SongViewsResponseViewModel> UpdateViewsAsync(int songId)
        {
            var songLyric = await _unitOfWork.GetRepository<SongLyric>()
                .GetAll(true).AsQueryable()
                .FirstOrDefaultAsync(x => x.SongId == songId);
            if (songLyric == null)
                return new SongViewsResponseViewModel(false, 0);

            songLyric.Views++;
            await _unitOfWork.SaveAsync();

            return new SongViewsResponseViewModel(true, songLyric.Views);
        }

        public async Task<bool> DeleteSongAsync(int id)
        {
            var result = await _songRepository.DeleteAsync(id);
            if (!result) return false;

            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
