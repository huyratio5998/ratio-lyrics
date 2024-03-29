namespace Ratio_Lyrics.Web.Constants
{
    public static class CommonConstant
    {
        public static double MaxOrderTimeExecuteMinutes = 5;
        public static string AppleMusic = "Apple Music";
        public static string Youtube = "Youtube";
        public static string Spotify = "Spotify";

        public static TimeSpan SlidingCacheExpireDefault = TimeSpan.FromMinutes(2);
        public static DateTimeOffset AbsoluteExpirationDefault = DateTime.Now.AddMinutes(5);

        public const string Admin = "Admin";
        public const string GoogleProvider = "Google";

        public const string Success = "Success";
        public const string Failure= "Failure";
    }
}
