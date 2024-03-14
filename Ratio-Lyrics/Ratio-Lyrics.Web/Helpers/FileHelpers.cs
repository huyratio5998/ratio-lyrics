using Ratio_Lyrics.Web.Constants;

namespace Ratio_Lyrics.Web.Helpers
{
    public static class FileHelpers
    {
        public static async Task<bool> UploadFile(IFormFile file, string wwwroot, string folderName1 = "", string folderName2 = "", string folderName3 = "", string folderName4 = "")
        {
            try
            {
                if (file == null) return false;
                
                var pathFolder = Path.Combine(wwwroot, folderName1, folderName2, folderName3, folderName4);

                // Determine whether the directory exists.
                if (!Directory.Exists(pathFolder))
                {
                    Directory.CreateDirectory(pathFolder);
                }

                var path = Path.Combine(pathFolder, file.FileName);
                if (File.Exists(path)) return true;

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static async Task<bool> UploadFiles(List<IFormFile> files, string wwwroot, string folderName1 = "", string folderName2 = "", string folderName3 = "", string folderName4 = "")
        {
            try
            {
                if (files == null || !files.Any()) return false;
                
                var pathFolder = Path.Combine(wwwroot, folderName1, folderName2, folderName3, folderName4);

                // Determine whether the directory exists.
                if (!Directory.Exists(pathFolder))
                {
                    Directory.CreateDirectory(pathFolder);
                }

                foreach (var file in files)
                {
                    var path = Path.Combine(pathFolder, file.FileName);
                    if (File.Exists(path)) continue;

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static string ResolveImage(string image)
        {
            if (string.IsNullOrWhiteSpace(image)) return string.Empty;
            return $"{FileConstants.ProductImageBaseUrl}/{image.Trim()}";
        }
    }
}
