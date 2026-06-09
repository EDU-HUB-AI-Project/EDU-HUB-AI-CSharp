using System.Text.Json;
using EDU_HUB_AI.Config;
using EDU_HUB_AI.Service;
using EDU_HUB_AI.exception;

namespace EDU_HUB_AI.Util
{
    // 시설 이미지 — ServerB 업로드, DB에는 path만 저장
    public static class FacilityImageStore
    {
        public const string WebPrefix = "/images/facility/";
        public const long MaxFileBytes = 10 * 1024 * 1024;

        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".webp" };

        private static readonly AdminFacilityInfoService UploadService = new();

        public static bool TryValidateSelectedFile(string path, out string error)
        {
            error = "";

            if (!File.Exists(path))
            {
                error = "파일을 찾을 수 없습니다.";
                return false;
            }

            var ext = Path.GetExtension(path);
            if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
            {
                error = "png, jpg, jpeg, webp 형식만 등록할 수 있습니다.";
                return false;
            }

            if (new FileInfo(path).Length > MaxFileBytes)
            {
                error = "10MB 이하 이미지만 등록할 수 있습니다.";
                return false;
            }

            return true;
        }

        // 이미지 업로드
        public static async Task<string> UploadSelectedFileAsync(string sourcePath)
        {
            if (!TryValidateSelectedFile(sourcePath, out var error))
                throw new InvalidOperationException(error);

            try
            {
                var response = await UploadService.UploadFacilityImage(sourcePath);
                if (response?.Status == 200 && !string.IsNullOrWhiteSpace(response.Data?.imagePath))
                    return response.Data.imagePath;

                throw new InvalidOperationException(response?.Message ?? "이미지 업로드에 실패했습니다.");
            }
            catch (ApiException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public static string? TryGetLocalPath(string? webPath)
        {
            if (string.IsNullOrWhiteSpace(webPath))
                return null;

            var relative = webPath.Trim().TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var webRoot = TryGetWebappRoot();
            if (webRoot == null)
                return null;

            var local = Path.Combine(webRoot, relative);
            return File.Exists(local) ? local : null;
        }

        public static Image? TryLoadThumbnail(string? webPath, int size = 48)
        {
            try
            {
                var local = TryGetLocalPath(webPath);
                if (local != null)
                    return ResizeImage(Image.FromFile(local), size);

                var url = TryGetImageUrl(webPath);
                if (url == null)
                    return null;

                using var client = new HttpClient();
                using var stream = client.GetStreamAsync(url).GetAwaiter().GetResult();
                using var img = Image.FromStream(stream);
                return ResizeImage(new Bitmap(img), size);
            }
            catch
            {
                return null;
            }
        }

        public static string? TryGetImageUrl(string? webPath)
        {
            if (string.IsNullOrWhiteSpace(webPath))
                return null;

            var baseUrl = TryGetApiBaseUrl()?.TrimEnd('/');
            if (string.IsNullOrEmpty(baseUrl))
                return null;

            var path = webPath.StartsWith('/') ? webPath : "/" + webPath;
            return baseUrl + path;
        }

        private static string? TryGetWebappRoot()
        {
            var dir = AppContext.BaseDirectory;
            for (var i = 0; i < 8; i++)
            {
                var candidate = Path.GetFullPath(Path.Combine(dir, "..", "EDU-HUB-AI-ServerB", "src", "main", "webapp"));
                if (Directory.Exists(candidate))
                    return candidate;

                candidate = Path.GetFullPath(Path.Combine(dir, "EDU-HUB-AI-ServerB", "src", "main", "webapp"));
                if (Directory.Exists(candidate))
                    return candidate;

                dir = Path.GetFullPath(Path.Combine(dir, ".."));
            }
            return null;
        }

        private static string? TryGetApiBaseUrl()
        {
            try
            {
                var configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
                if (!File.Exists(configPath))
                    return null;

                var config = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(configPath));
                return config?.ApiSettings?.BaseUrl;
            }
            catch
            {
                return null;
            }
        }

        private static Image ResizeImage(Image src, int size)
        {
            var bmp = new Bitmap(size, size);
            using var g = Graphics.FromImage(bmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(src, 0, 0, size, size);
            return bmp;
        }
    }
}
