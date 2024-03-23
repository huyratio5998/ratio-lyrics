using Ratio_Lyrics.Web.Models.Recaptcha;

namespace Ratio_Lyrics.Web.Models
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            
        }
        public HomeViewModel(SongViewModel? songModel, CaptchaViewModel captchaSettings)
        {
            SongModel = songModel;
            CaptchaSettings = captchaSettings;
        }

        public string PublicApiUrl { get; set; }
        public SongViewModel? SongModel { get; set; }
        public CaptchaViewModel CaptchaSettings { get; set; }
    }
}
