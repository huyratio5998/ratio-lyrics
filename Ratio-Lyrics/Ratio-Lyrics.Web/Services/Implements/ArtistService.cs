using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Repositories.Abstracts;
using Ratio_Lyrics.Web.Services.Abstraction;

namespace Ratio_Lyrics.Web.Services.Implements
{
    public class ArtistService : IArtistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<ArtistViewModel> _validator;
        private IBaseRepository<Artist> _artistRepository;
        public ArtistService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<ArtistViewModel> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
            _artistRepository = _unitOfWork.GetRepository<Artist>();
        }

        public async Task<ArtistViewModel?> GetArtist(int artistId, bool isTracking = true)
        {
            if (artistId == 0) return null;

            var artist = await _artistRepository.GetByIdAsync(artistId, isTracking);
            if (artist == null) return null;

            return _mapper.Map<ArtistViewModel>(artist);
        }

        public async Task<ArtistViewModel?> GetArtist(string name, bool isTracking = true)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var artist = await _artistRepository
                .GetAll(isTracking).AsQueryable()
                .FirstOrDefaultAsync(x=>x.Name.Equals(name));
            if (artist == null) return null;

            return _mapper.Map<ArtistViewModel>(artist);
        }

        public async Task<PagedResponse<ArtistViewModel>> GetArtistsAsync(BaseQueryParams queryParams)
        {
            IQueryable<Artist> items = _artistRepository
                            .GetAll().AsQueryable();

            // filter
            if (!string.IsNullOrWhiteSpace(queryParams.SearchText))
                items = items.Where(x => x.Name.Contains(queryParams.SearchText));

            // order
            var orderCondition = queryParams.OrderBy;
            if (orderCondition != null)
            {
                if (orderCondition == OrderType.Asc) items = items.OrderBy(x => x.Id);
                else if (orderCondition == OrderType.Desc) items = items.OrderByDescending(y => y.Id);
            }

            // paging
            queryParams.PageNumber = queryParams.PageNumber <= 0 ? CommonConstant.PageIndexDefault : queryParams.PageNumber;
            queryParams.PageSize = queryParams.PageSize <= 0 ? CommonConstant.PageSizeDefault : queryParams.PageSize;

            var artists = await PagedResponse<Artist>.CreateAsync(items, queryParams.PageNumber, queryParams.PageSize);

            var result = _mapper.Map<PagedResponse<ArtistViewModel>>(artists);
            return result;
        }

        public async Task<Artist?> CreateArtistRequestAsync(ArtistViewModel newArtist)
        {
            var validateResult = await _validator.ValidateAsync(newArtist);
            if (!validateResult.IsValid) return null;

            var result = await _artistRepository.CreateAsync(_mapper.Map<Artist>(newArtist));           
            return result;
        }

        public async Task<int> CreateArtistAsync(ArtistViewModel newArtist)
        {
            var result = await CreateArtistRequestAsync(newArtist);
            await _unitOfWork.SaveAsync();

            if (result == null || result.Id == 0) return 0;
            return result.Id;
        }

        public async Task<bool> UpdateArtistAsync(ArtistViewModel newArtist)
        {
            var validateResult = await _validator.ValidateAsync(newArtist);
            if (!validateResult.IsValid) return false;

            var result = _artistRepository.Update(_mapper.Map<Artist>(newArtist));
            await _unitOfWork.SaveAsync();
            return result;
        }

        public async Task<bool> DeleteArtistAsync(int ArtistId)
        {
            var result = await _artistRepository.DeleteAsync(ArtistId);
            if (!result) return false;

            await _unitOfWork.SaveAsync();
            return result;
        }
    }
}
