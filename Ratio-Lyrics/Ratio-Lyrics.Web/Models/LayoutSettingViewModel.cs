using Microsoft.AspNetCore.Identity;
using Ratio_Lyrics.Web.Entities;
using Ratio_Lyrics.Web.Services.Abstraction;

namespace Ratio_Lyrics.Web.Models
{
    public class LayoutSettingsViewModel : ILayoutSettingsViewModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISiteSettingService _siteSettingService;
        private readonly IConfiguration _configuration;       

        public LayoutSettingsViewModel(IHttpContextAccessor httpContextAccessor, ISiteSettingService siteSettingService, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _siteSettingService = siteSettingService;
            _configuration = configuration;
            PublicApiUrl = _configuration["PublicApiUrl"];            
            //this.SiteSettings = _siteSettingService.GetSiteSetting()?.Result;
            //_adminSiteSettings = _siteSettingService.GetSiteSetting(true)?.Result;
        }
        private SiteSettingViewModel? SiteSettings;
        private SiteSettingViewModel? _adminSiteSettings;

        public string PublicApiUrl { get; set;}
        
        //public string StoreName => SiteSettings?.GeneralSetting?.SiteName ?? CommonConstant.StoreName;

        //public string StoreIcon => SiteSettings?.HeaderSetting?.ShopLogo?.Icon?.ImageSrc ?? "/images/icons/logo-01.png";

        //public string StoreLogo => SiteSettings?.GeneralSetting?.SiteLogo?.ImageSrc ?? "/images/icons/favicon.png";

        //SiteSettingViewModel ILayoutSettingsViewModel.SiteSettings => this.SiteSettings;

        //public SiteSettingViewModel AdminSiteSettings => this._adminSiteSettings;

        public string CurrentPath()
        {
            return _httpContextAccessor.HttpContext?.Request?.Path.ToString() ?? String.Empty;
        }

        //public FooterSettingsViewModel FooterSettings()
        //{
        //    return new FooterSettingsViewModel
        //    {
        //        FooterSetting = SiteSettings?.FooterSetting,
        //    };
        //}

        //public HeaderSettingsViewModel HeaderSettings()
        //{
        //    var currentPath = CurrentPath();
        //    var headerSettings = new HeaderSettingsViewModel()
        //    {
        //        HeaderSetting = SiteSettings?.HeaderSetting,
        //        HeaderSlides = SiteSettings?.HeaderSlides,
        //        IsHideSilder = HideSliderByPath(currentPath)
        //    };
        //    return headerSettings;
        //}        

        //public CommonSettingsViewModel CommonSettings()
        //{
        //    var currentPath = CurrentPath();
        //    var commonSettings = new CommonSettingsViewModel
        //    {
        //        IsHideRegisterPopup = HideRegisterPopup(currentPath)
        //    };
        //    return commonSettings;
        //}
    }
}
