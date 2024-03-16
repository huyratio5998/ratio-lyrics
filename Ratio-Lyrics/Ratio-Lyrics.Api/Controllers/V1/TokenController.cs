using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ratio_Lyrics.Api.Models;
using Ratio_Lyrics.Api.Services;
using Ratio_Lyrics.Web.Services.Abstractions;

namespace Ratio_Lyrics.Api.Controllers.V1
{
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ICommonService _commonService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenController> _logger;

        private const string JwtAdmin = "Jwt:Admin";
        private const string JwtHashPass = "Jwt:HashPass";
        private const string JwtHashSalt = "Jwt:HashSalt";

        public TokenController(ITokenService tokenService, ICommonService commonService, IConfiguration configuration, ILogger<TokenController> logger)
        {
            _tokenService = tokenService;
            _commonService = commonService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CredentialViewModel credential)
        {
            if (credential == null || string.IsNullOrWhiteSpace(credential.UserName) || string.IsNullOrWhiteSpace(credential.Password)) return BadRequest();

            if (!VerifyUser(credential)) return BadRequest();

            var token = await Task.Run(() => _tokenService.CreateToken(credential.UserName, credential.Password));
            return Ok(token);
        }

        private bool VerifyUser(CredentialViewModel credential)
        {
            var acceptedUser = _configuration[JwtAdmin];
            var hashPass = _configuration[JwtHashPass];
            var hashSalt = _configuration[JwtHashSalt];

            if (string.IsNullOrWhiteSpace(acceptedUser)
                || string.IsNullOrWhiteSpace(hashPass)
                || string.IsNullOrWhiteSpace(hashSalt))
            {
                _logger.LogError("Jwt account missing information");
                return false;
            }

            if (!credential.UserName.Equals(acceptedUser)) return false;

            return _commonService.VerifyPassword(credential.Password, hashPass, Convert.FromHexString(hashSalt));
        }
    }
}
