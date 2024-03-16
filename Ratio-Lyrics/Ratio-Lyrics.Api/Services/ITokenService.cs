using Ratio_Lyrics.Api.Models;

namespace Ratio_Lyrics.Api.Services
{
    public interface ITokenService
    {
        TokenResponse CreateToken(string userName, string password);
    }
}
