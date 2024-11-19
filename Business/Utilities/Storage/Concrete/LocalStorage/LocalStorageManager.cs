using Business.Utilities.Storage.Abstract.LocalStorage;
using Entities.DTOs.UploadDTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using static System.IO.Path;

namespace Business.Utilities.Storage.Concrete.LocalStorage;

public class LocalStorageManager(IWebHostEnvironment env) : ILocalStorageService
{
    public async Task<UploadDto> UploadFileAsync(string containerName, IFormFile file)
    {
        string uploadPath = Combine(env.WebRootPath, containerName);

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var newFileName = Guid.NewGuid() + file.FileName;
        var path = Combine(uploadPath, newFileName);

        if (GetExtension(file.FileName).ToLower() == ".svg")
        {
            await CopyFileAsync(path, file);
        }
        else
        {
            await CopyFileAsync(path, file);
            await CompressAndSaveImageAsync(file, path);
        }

        return new UploadDto
        {
            FileName = newFileName,
            Path = path
        };
    }

    public async Task<List<UploadDto>> UploadFilesAsync(string containerName, IFormFileCollection files)
    {
        var uploadList = new List<UploadDto>();
        foreach (var file in files)
        {
            var uploadDto = await UploadFileAsync(containerName, file);
            uploadList.Add(uploadDto);
        }

        return uploadList;
    }

    public async Task DeleteAsync(string pathOrContainerName, string fileName)
        => File.Delete($"{pathOrContainerName}\\{fileName}");

    public Task<List<string>> GetAllFilesAsync(string? pathOrContainerName = null)
    {
        if (pathOrContainerName == null)
        {
            var wwwrootPath = Path.Combine(env.WebRootPath);
            var files = GetFilesFromDirectory(wwwrootPath);
            return Task.FromResult(files);
        }
        var path = Path.Combine(env.WebRootPath, pathOrContainerName);
        DirectoryInfo directoryInfo = new(path);
        return Task.FromResult(directoryInfo.GetFiles().Select(x => x.Name).ToList());
    }

    private List<string> GetFilesFromDirectory(string directory)
    {
        var filesList = new List<string>();
        var files = Directory.GetFiles(directory);
        var directories = Directory.GetDirectories(directory);

        foreach (var file in files)
        {
            filesList.Add(file.Replace(env.WebRootPath, "").Replace("\\", "/"));
        }

        foreach (var dir in directories)
        {
            filesList.AddRange(GetFilesFromDirectory(dir));
        }

        return filesList;
    }
    static async Task<bool> CopyFileAsync(string filePath, IFormFile file)
    {
        try
        {
            await using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: true);
            await file.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
            return true;
        }
        catch (Exception)
        {
            //todo log
            return false;
        }
    }
    private static async Task CompressAndSaveImageAsync(IFormFile file, string outputPath)
    {
        try
        {
            using var image = await SixLabors.ImageSharp.Image.LoadAsync(file.OpenReadStream());
            var extension = GetExtension(file.FileName).ToLower();
            // Check the image format and set the appropriate encoder
            IImageEncoder encoder = extension switch
            {
                ".jpg" or ".jpeg" => new JpegEncoder
                {
                    Quality = 75 // Adjust the quality to reduce the file size
                },
                ".png" => new PngEncoder
                {
                    CompressionLevel = PngCompressionLevel.BestCompression // Adjust the compression level
                },
                ".bmp" => new BmpEncoder(),
                ".gif" => new GifEncoder(),
                _ => throw new InvalidOperationException("Unsupported image format."),
            };

            // Save the image with the specified encoder
            await image.SaveAsync(outputPath, encoder);
        }
        catch (UnknownImageFormatException)
        {
            throw new InvalidOperationException("Unsupported image format.");
        }
    }
}