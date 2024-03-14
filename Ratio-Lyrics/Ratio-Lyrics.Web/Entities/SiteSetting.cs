namespace Ratio_Lyrics.Web.Entities
{
    public class SiteSetting : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Value { get; set; }
        public SettingType SettingType { get; set; }
        public bool IsActive { get; set; }
    }

    public enum SettingType
    {
        Header = 0,
        Footer = 1,
        SEO = 2,
        Feature = 3
    }
}
