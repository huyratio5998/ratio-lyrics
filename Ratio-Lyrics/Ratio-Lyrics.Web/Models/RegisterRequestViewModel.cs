using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ratio_Lyrics.Web.Models
{
    public class RegisterRequestViewModel
    {
        [JsonPropertyName("userName")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Special characters are not allowed")]
        public string? UserName { get; set; }
        [JsonPropertyName("password")]
        public string? Password { get; set; }
        public string? DisplayName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? SaltHash { get; set; }
    }
}
