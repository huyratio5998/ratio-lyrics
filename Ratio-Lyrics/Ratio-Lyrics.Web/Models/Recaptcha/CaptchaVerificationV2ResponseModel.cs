using System.Text.Json.Serialization;

namespace Ratio_Lyrics.Web.Models.Recaptcha
{
    public class CaptchaVerificationV2ResponseModel
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("challenge_ts")]
        public DateTime Date { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        public override string ToString() => $"{nameof(Success)}: {Success}, {nameof(Date)}: {Date}, {nameof(Hostname)}: {Hostname}";
    }
}
