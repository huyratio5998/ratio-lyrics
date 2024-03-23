using Microsoft.Extensions.Options;
using Ratio_Lyrics.Web.Models.Recaptcha;
using Ratio_Lyrics.Web.Services.Abstraction;
using System.Text.Json;

namespace Ratio_Lyrics.Web.Services.Implements
{
    public class GoogleCaptchaService : ICaptchaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GoogleCaptchaOptions _options;

        public GoogleCaptchaService(IHttpClientFactory httpClientFactory, IOptions<GoogleCaptchaOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public async Task<CaptchaVerificationV2ResponseModel?> VerifyV2Async(string token)
        {
            using var httpClient = _httpClientFactory.CreateClient(GoogleCaptchaConstants.GoogleCaptcha);
            string verification = await httpClient.GetStringAsync($"recaptcha/api/siteverify?secret={_options.Secret}&response={token}");

            return JsonSerializer.Deserialize<CaptchaVerificationV2ResponseModel>(verification);
        }

        public async Task<CaptchaVerificationV3ResponseModel?> VerifyV3Async(CaptchaVerificationRequestModel request)
        {
            var requestInput = new Dictionary<string, string>
        {
            { "secret", _options.Secret },
            { "response", request.Token }
        };

            if (request.HasRemoteIp())
                requestInput.Add("remoteip", request.RemoteIp);

            using var httpClient = _httpClientFactory.CreateClient(GoogleCaptchaConstants.GoogleCaptcha);
            HttpResponseMessage response = await httpClient.PostAsync("recaptcha/api/siteverify", new FormUrlEncodedContent(requestInput));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CaptchaVerificationV3ResponseModel>();
        }
    }
}
