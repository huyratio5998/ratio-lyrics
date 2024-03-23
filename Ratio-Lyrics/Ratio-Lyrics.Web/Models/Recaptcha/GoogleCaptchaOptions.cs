namespace Ratio_Lyrics.Web.Models.Recaptcha
{
    public class GoogleCaptchaOptions
    {
        public bool Enabled { get; set; }
        public string SiteKey { get; set; }
        public string Secret { get; set; }
        public string Action { get; set; }
        public decimal ScoreThreshold { get; set; }
        public string Version { get; set; }

        public bool IsVersion2() => Version == GoogleCaptchaConstants.Version_2;
        public bool IsVersion3() => Version == GoogleCaptchaConstants.Version_3;

        /// <inheritdoc />
        public override string ToString() => $"{nameof(Enabled)}: {Enabled}, {nameof(SiteKey)}: {SiteKey}, {nameof(Action)}: {Action}, {nameof(ScoreThreshold)}: {ScoreThreshold}, {nameof(Version)}: {Version}";
    }
}
