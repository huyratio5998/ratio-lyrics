
namespace Ratio_Lyrics.Web.Models.Recaptcha
{
    public class CaptchaVerificationRequestModel
    {
        public string Token { get; set; }
        public string RemoteIp { get; set; }

        public bool HasRemoteIp() => !string.IsNullOrEmpty(RemoteIp);        
    }
}
