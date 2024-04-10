using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Models.Recaptcha;
using Ratio_Lyrics.Web.Services.Abstraction;
using System.Diagnostics;

namespace Ratio_Lyrics.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISongService _songService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMediaPlatformService _mediaPlatformService;
        private readonly IMapper _mapper;
        private readonly GoogleCaptchaOptions _googleCaptchaOptions;
        private readonly ICaptchaService _captchaService;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, ISongService songService, IMediaPlatformService mediaPlatformService, IMapper mapper, IOptions<GoogleCaptchaOptions> googleCaptchaOptions, ICaptchaService captchaService, IConfiguration configuration, IAuthenticationService authenticationService)
        {
            _logger = logger;
            _songService = songService;
            _mediaPlatformService = mediaPlatformService;
            _mapper = mapper;
            _googleCaptchaOptions = googleCaptchaOptions.Value;
            _captchaService = captchaService;
            _configuration = configuration;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string text)
        {
            var captchaSetting = new CaptchaViewModel(_googleCaptchaOptions);
            if (string.IsNullOrEmpty(text))
            {
                var medias = await Task.Run(() => _mediaPlatformService.GetMediaPlatformsAsync());
                var songModel = new SongViewModel
                {
                    MediaPlatformLinks = _mapper.Map<List<SongMediaPlatformViewModel>>(medias),
                };
                var model = new HomeViewModel(songModel, captchaSetting);
                model.PublicApiUrl = _configuration["PublicApiUrl"]?.ToString() ?? "https://localhost:7186";

                return View(model);
            }

            var song = await _songService.GetSongAsync(text, false);
            var homeViewModel = new HomeViewModel(song, captchaSetting);
            homeViewModel.PublicApiUrl = _configuration["PublicApiUrl"]?.ToString() ?? "https://localhost:7186";

            return View(homeViewModel);
        }

        private async Task<bool> RecaptchaValidation()
        {
            if (_googleCaptchaOptions == null || !_googleCaptchaOptions.Enabled) return true;

            string googleCaptchaResponse = Request.Form["g-recaptcha-response"].ToString();
            if (string.IsNullOrEmpty(googleCaptchaResponse))
            {
                _logger.LogError("Please verify the captcha before continue.");
                return false;
            }

            if (_googleCaptchaOptions.IsVersion2())
            {
                CaptchaVerificationV2ResponseModel response = await _captchaService.VerifyV2Async(googleCaptchaResponse);
                if (!response.Success)
                {
                    _logger.LogError("Request doesn't match the minimum security requirements. Please try again later.");
                    return false;
                }
            }
            else if (_googleCaptchaOptions.IsVersion3())
            {
                var request = new CaptchaVerificationRequestModel
                {
                    Token = googleCaptchaResponse,
                    RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString()
                };

                CaptchaVerificationV3ResponseModel response = await _captchaService.VerifyV3Async(request);

                if (!response.Success)
                {
                    _logger.LogError("Something went wrong with your verification request. Please try again later.");
                    return false;
                }
                else
                {
                    // it was a success captcha response
                    // doesn't mean the scope is great
                    // this step here is optional. You can have multiple actions within the same application.
                    // And by action, it can be: login, reset password, change password (being logged in), etc.
                    // Being so, doesn't make sense to be verified by setting but by context instead

                    if (!response.Action.Equals(_googleCaptchaOptions.Action, StringComparison.InvariantCulture))
                    {
                        _logger.LogError("Site action mismatch. Please try again later.");
                        return false;
                    }

                    if (response.Score < _googleCaptchaOptions.ScoreThreshold)
                    {
                        _logger.LogError("Request doesn't match the minimum security requirements. Please try again later.");
                        return false;
                    }
                }
            }
            else // _googleCaptchaOptions.Version is invalid
            {
                _logger.LogError("Request doesn't match the minimum security requirements. Please try again later.");
                return false;
            }

            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomeViewModel homeViewModel)
        {
            var recaptchaValidation = await RecaptchaValidation();
            if (!recaptchaValidation) return RedirectToAction(nameof(Index));

            var newSong = homeViewModel.SongModel;
            newSong.SearchKey = _songService.BuildSearchKey(newSong);            
                        
            var currentUser = await _authenticationService.GetCurrentUser(User);
            newSong.ContributedBy = currentUser?.DisplayName ?? User?.Identity?.Name;

            var result = await _songService.CreateSongAsync(newSong);
            if (result <= 0) return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Index), new { text = newSong.Name });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
