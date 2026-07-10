namespace Admin.Services;

/// <summary>
/// Saves uploaded images so BOTH the Admin portal and the public Website can serve them.
/// Files are written to Admin's wwwroot/uploads and mirrored to the Web project's
/// wwwroot/uploads (path resolved relative to the solution or overridden via
/// configuration key "Uploads:WebRoot"). Stored URLs are relative ("/uploads/...")
/// so they resolve on whichever site renders them.
/// </summary>
public interface IImageUploadService
{
    /// <summary>Saves the image and returns its relative URL, or null when file is empty/invalid.</summary>
    Task<string?> SaveAsync(IFormFile? file, string subfolder);
}

public class ImageUploadService : IImageUploadService
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
    private const long MaxBytes = 10 * 1024 * 1024; // 10 MB

    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;
    private readonly ILogger<ImageUploadService> _logger;

    public ImageUploadService(IWebHostEnvironment env, IConfiguration config, ILogger<ImageUploadService> logger)
    {
        _env = env;
        _config = config;
        _logger = logger;
    }

    public async Task<string?> SaveAsync(IFormFile? file, string subfolder)
    {
        if (file == null || file.Length == 0 || file.Length > MaxBytes)
        {
            return null;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return null;
        }

        subfolder = string.Join("", subfolder.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'));
        var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..34] + extension;
        var relativePath = Path.Combine("uploads", subfolder, fileName);

        // Primary: Admin wwwroot (previews inside the portal).
        var adminPath = Path.Combine(_env.WebRootPath, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(adminPath)!);
        await using (var stream = File.Create(adminPath))
        {
            await file.CopyToAsync(stream);
        }

        // Mirror: Web wwwroot so the public site can serve the same URL.
        var webRoot = _config["Uploads:WebRoot"];
        if (string.IsNullOrWhiteSpace(webRoot))
        {
            webRoot = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "Web", "wwwroot"));
        }

        try
        {
            if (Directory.Exists(Path.GetDirectoryName(webRoot)))
            {
                var webPath = Path.Combine(webRoot, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(webPath)!);
                File.Copy(adminPath, webPath, overwrite: true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not mirror upload {File} to the Web project", fileName);
        }

        return "/" + relativePath.Replace('\\', '/');
    }
}
