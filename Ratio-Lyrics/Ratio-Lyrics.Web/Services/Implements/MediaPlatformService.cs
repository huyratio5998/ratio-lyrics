using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Repositories.Abstracts;
using Ratio_Lyrics.Web.Services.Abstraction;

namespace Ratio_Lyrics.Web.Services.Implements
{
    public class MediaPlatformService : IMediaPlatformService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<MediaPlatformViewModel> _validator;
        private IBaseRepository<MediaPlatform> _mediaPlatformRepository;
        public MediaPlatformService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<MediaPlatformViewModel> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
            _mediaPlatformRepository = _unitOfWork.GetRepository<MediaPlatform>();
        }

        public async Task<MediaPlatformViewModel?> GetMediaPlatform(int mediaPlatformId, bool isTracking = true)
        {
            if (mediaPlatformId == 0) return null;

            var mediaPlatform = await _mediaPlatformRepository.GetByIdAsync(mediaPlatformId, isTracking);
            if (mediaPlatform == null) return null;

            return _mapper.Map<MediaPlatformViewModel>(mediaPlatform);
        }

        public async Task<MediaPlatformViewModel?> GetMediaPlatform(string name, bool isTracking = true)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var mediaPlatForm = await _mediaPlatformRepository
                .GetAll(isTracking).AsQueryable()
                .FirstOrDefaultAsync(x => x.Name.Equals(name));

            return _mapper.Map<MediaPlatformViewModel>(mediaPlatForm);
        }

        public List<MediaPlatformViewModel> GetMediaPlatformsAsync()
        {
            var items = _mediaPlatformRepository.GetAll().ToList();
            var result = _mapper.Map<List<MediaPlatformViewModel>>(items);
            return result;
        }

        public async Task<int> CreateMediaPlatformAsync(MediaPlatformViewModel newMediaPlatform)
        {
            var validateResult = await _validator.ValidateAsync(newMediaPlatform);
            if (!validateResult.IsValid) return 0;

            var result = await _mediaPlatformRepository.CreateAsync(_mapper.Map<MediaPlatform>(newMediaPlatform));
            if (result == null || result.Id == 0) return 0;

            await _unitOfWork.SaveAsync();
            return result.Id;
        }

        public async Task<bool> UpdateMediaPlatformAsync(MediaPlatformViewModel newMediaPlatform)
        {
            var validateResult = await _validator.ValidateAsync(newMediaPlatform);
            if (!validateResult.IsValid) return false;

            var result = _mediaPlatformRepository.Update(_mapper.Map<MediaPlatform>(newMediaPlatform));
            if (!result) return false;

            await _unitOfWork.SaveAsync();
            return result;
        }

        public async Task<bool> DeleteMediaPlatformAsync(int MediaPlatformId)
        {
            var result = await _mediaPlatformRepository.DeleteAsync(MediaPlatformId);
            if (!result) return false;

            await _unitOfWork.SaveAsync();
            return result;
        }       
    }
}
