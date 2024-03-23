using Ratio_Lyrics.Web.Models.Recaptcha;

namespace Ratio_Lyrics.Web.Services.Abstraction
{
    public interface ICaptchaService
    {
        Task<CaptchaVerificationV2ResponseModel?> VerifyV2Async(string token);
        Task<CaptchaVerificationV3ResponseModel?> VerifyV3Async(CaptchaVerificationRequestModel request);
    }
}
