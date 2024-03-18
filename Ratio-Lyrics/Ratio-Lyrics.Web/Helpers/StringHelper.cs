using System.Globalization;

namespace Ratio_Lyrics.Web.Helpers
{
    public static class StringHelper
    {
        public static string FormatViews(int num)
        {
            if (num > 999999999 || num < -999999999)
            {
                return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999999 || num < -999999)
            {
                return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999 || num < -999)
            {
                return num.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
