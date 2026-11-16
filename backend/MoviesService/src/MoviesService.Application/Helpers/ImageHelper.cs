using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MoviesService.Application.Helpers
{
    /// <summary>
    /// Helper utility for handling server-side file uploads and storage.
    /// </summary>
    public class ImageHelper(IWebHostEnvironment env)
    {
        private readonly IWebHostEnvironment _env = env;

        /// <summary>
        /// Saves an uploaded form file to the specified web root folder.
        /// </summary>
        /// <param name="file">The uploaded file.</param>
        /// <param name="folder">The target folder name inside wwwroot.</param>
        /// <returns>The relative file path, or null if the file is empty.</returns>
        public string? SaveImage(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            var path = Path.Combine(_env.WebRootPath, folder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return $"/{folder}/{fileName}";
        }
    }
}