namespace Ratio_Lyrics.Web.Services.Abstractions
{
    public interface ICommonService
    {
        string HashPasword(string password, out byte[] salt);
        bool VerifyPassword(string password, string hash, byte[] salt);
    }
}
