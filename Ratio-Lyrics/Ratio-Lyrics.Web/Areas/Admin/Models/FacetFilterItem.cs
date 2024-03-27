using System.Text.Json.Serialization;

namespace Ratio_Lyrics.Web.Areas.Admin.Models
{
    public class FacetFilterItem
    {
        [JsonPropertyName("fieldName")]
        public string FieldName { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
