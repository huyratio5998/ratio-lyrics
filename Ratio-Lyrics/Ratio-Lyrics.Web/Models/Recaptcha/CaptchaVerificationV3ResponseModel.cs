using System.Text.Json.Serialization;

namespace Ratio_Lyrics.Web.Models.Recaptcha
{
    public class CaptchaVerificationV3ResponseModel : CaptchaVerificationV2ResponseModel
    {
        [JsonPropertyName("score")]
        public decimal Score { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        public override string ToString() => $"{base.ToString()}, {nameof(Score)}: {Score}, {nameof(Action)}: {Action}";
    }
}
