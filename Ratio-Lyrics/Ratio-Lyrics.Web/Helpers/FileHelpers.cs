namespace Ratio_Lyrics.Web.Helpers
{
    public static class FileHelpers
    {
        public static async Task<bool> UploadFile(IFormFile file, string fileNameSave, string wwwroot, string folderName1 = "", string folderName2 = "", string folderName3 = "", string folderName4 = "")
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
                
                var path = Path.Combine(pathFolder, fileNameSave);
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
                    var path = Path.Combine(pathFolder, file.Name);
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

        public static string FileNameFormatDateTime(string fileName)
        {
            return $"{Path.GetFileNameWithoutExtension(fileName)}{DateTime.UtcNow.ToString("_yyyy_MM_dd_HH_mm_ss")}{Path.GetExtension(fileName)}";
        }

        public static string ResolveImage(string baseUrl, string image)
        {
            if (string.IsNullOrWhiteSpace(image)) return string.Empty;
            return $"/{baseUrl}/{image.Trim()}";
        }
    }
}
