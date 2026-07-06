using CollegeWebSite.Data;
using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Domain.Enums;
using CollegeWebSite.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

namespace CollegeWebSite.Services.Implementation;

public class MediaService : IMediaService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<MediaService> _logger;

    public MediaService(IDbContextFactory<ApplicationDbContext> contextFactory, 
        IWebHostEnvironment env,
        ILogger<MediaService> logger)
    {
        _contextFactory = contextFactory;
        _env = env;
        _logger = logger;
    }

    public async Task<List<Media>> GetAllMediaAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Media.OrderByDescending(m => m.CreatedOn).ToListAsync();
    }

    public async Task<Media?> GetMediaByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Media.FindAsync(id);
    }

    public async Task<Media?> UploadMediaAsync(IBrowserFile file, string? altText = null, string? description = null)
    {
        try
        {
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var extension = Path.GetExtension(file.Name).ToLower();
            var contentType = file.ContentType;
            var isImage = contentType.StartsWith("image/") && extension != ".svg" && extension != ".gif";
            
            var safeFileName = $"{Guid.NewGuid()}{(isImage ? ".webp" : extension)}";
            var filePath = Path.Combine("uploads", safeFileName);
            var fullPath = Path.Combine(_env.WebRootPath, filePath);

            int? width = null;
            int? height = null;
            long fileSize = file.Size;

            if (isImage)
            {
                using var imageStream = file.OpenReadStream(maxAllowedSize: 20 * 1024 * 1024);
                using var memoryStream = new MemoryStream();
                await imageStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var image = await Image.LoadAsync(memoryStream);
                width = image.Width;
                height = image.Height;

                // Simple optimization: Resize if too large (e.g., > 2000px)
                if (width > 2000 || height > 2000)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(2000, 2000),
                        Mode = ResizeMode.Max
                    }));
                    width = image.Width;
                    height = image.Height;
                }

                await image.SaveAsync(fullPath, new WebpEncoder { Quality = 80 });
                fileSize = new FileInfo(fullPath).Length;
                contentType = "image/webp";
            }
            else
            {
                await using var fs = new FileStream(fullPath, FileMode.Create);
                await file.OpenReadStream(maxAllowedSize: 50 * 1024 * 1024).CopyToAsync(fs);
            }

            var media = new Media
            {
                FileName = file.Name,
                FilePath = filePath,
                Url = $"/{filePath.Replace('\\', '/')}",
                MimeType = contentType,
                FileSize = fileSize,
                AltText = altText ?? Path.GetFileNameWithoutExtension(file.Name),
                Description = description,
                Width = width,
                Height = height,
                Type = GetMediaType(contentType),
                CreatedOn = DateTime.UtcNow
            };

            using var context = await _contextFactory.CreateDbContextAsync();
            context.Media.Add(media);
            await context.SaveChangesAsync();

            return media;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while uploading media asset: {FileName}", file.Name);
            return null;
        }
    }

    public async Task<bool> DeleteMediaAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var media = await context.Media.FindAsync(id);
        if (media == null) return false;

        media.DeletedOn = DateTime.UtcNow;
        context.Media.Update(media);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateMediaInfoAsync(long id, string? altText, string? description)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var media = await context.Media.FindAsync(id);
        if (media == null) return false;

        media.AltText = altText;
        media.Description = description;
        media.LastModifiedOn = DateTime.UtcNow;

        context.Media.Update(media);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<int> GetTotalMediaCountAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Media.CountAsync();
    }

    private MediaType GetMediaType(string contentType)
    {
        if (contentType.StartsWith("image/")) return MediaType.Image;
        if (contentType.StartsWith("video/")) return MediaType.Video;
        if (contentType.Contains("pdf") || contentType.Contains("word") || contentType.Contains("excel") || contentType.Contains("spreadsheet")) return MediaType.Document;
        return MediaType.Other;
    }
}
