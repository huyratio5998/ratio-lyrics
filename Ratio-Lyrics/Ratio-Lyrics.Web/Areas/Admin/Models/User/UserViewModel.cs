using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ratio_Lyrics.Web.Areas.Admin.Models.User
{
    public class UserViewModel
    {
        [JsonPropertyName("userName")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Special characters are not allowed")]
        public string? UserName { get; set; }
        [JsonPropertyName("password")]
        public string? Password { get; set; }
        public string? HashSalt { get; set; }

        public Guid Id { get; set; }
        public string? DisplayName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public List<string> UserRoles { get; set; } = new List<string>();
        public List<string>? AvailableRoles { get; set; }

    }
}
