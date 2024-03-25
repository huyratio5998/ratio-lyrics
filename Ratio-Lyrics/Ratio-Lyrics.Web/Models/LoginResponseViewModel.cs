using System.Text.Json.Serialization;

namespace Ratio_Lyrics.Web.Models
{
    public class LoginResponseViewModel
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
