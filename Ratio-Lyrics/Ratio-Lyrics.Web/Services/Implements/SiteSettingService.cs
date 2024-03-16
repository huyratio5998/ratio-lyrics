using AutoMapper;
using FluentValidation;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Repositories.Abstracts;
using Ratio_Lyrics.Web.Services.Abstraction;

namespace Ratio_Lyrics.Web.Services.Implements
{
    public class SiteSettingService : ISiteSettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<SiteSettingViewModel> _validator;
        private IBaseRepository<SiteSetting> _siteSettingRepository;
        public SiteSettingService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<SiteSettingViewModel> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
            _siteSettingRepository = _unitOfWork.GetRepository<SiteSetting>();
        }

        public async Task<int> Create(SiteSettingViewModel setting)
        {
            var validateResult = await _validator.ValidateAsync(setting);
            if (!validateResult.IsValid) return 0;

            var result = await _siteSettingRepository.CreateAsync(_mapper.Map<SiteSetting>(setting));
            await _unitOfWork.SaveAsync();
            return result.Id;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await _siteSettingRepository.DeleteAsync(id);
            if (!result) return false;

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<SiteSettingViewModel?> Get(int id, bool isTracking = true)
        {
            if (id == 0) return null;
            var setting = await _siteSettingRepository.GetByIdAsync(id);
            if (setting == null) return null;

            return _mapper.Map<SiteSettingViewModel>(setting);
        }

        public async Task<List<SiteSettingViewModel>> Gets()
        {
            var results = await Task.Run(() => _siteSettingRepository.GetAll().ToList());
            return _mapper.Map<List<SiteSettingViewModel>>(results);
        }

        public async Task<bool> Update(SiteSettingViewModel setting)
        {
            var target = await _siteSettingRepository.GetByIdAsync(setting.Id, false);
            if (target == null) return false;

            var result = _siteSettingRepository.Update(_mapper.Map<SiteSetting>(setting));
            if (!result) return false;

            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
