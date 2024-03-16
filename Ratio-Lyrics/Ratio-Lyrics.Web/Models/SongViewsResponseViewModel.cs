namespace Ratio_Lyrics.Web.Models
{
    public class SongViewsResponseViewModel
    {
        public SongViewsResponseViewModel(bool updateSuccess, decimal views)
        {
            UpdateSuccess = updateSuccess;
            Views = views;
        }

        public bool UpdateSuccess { get; set; }
        public decimal Views { get; set; }
    }
}
