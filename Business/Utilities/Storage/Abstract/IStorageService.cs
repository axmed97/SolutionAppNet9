using Entities.DTOs.UploadDTOs;
using Microsoft.AspNetCore.Http;

namespace Business.Utilities.Storage;

public interface IStorageService
{
    Task<UploadDto> UploadFileAsync(string containerName, IFormFile file);
    Task<List<UploadDto>> UploadFilesAsync(string containerName, IFormFileCollection files);
    Task DeleteAsync(string pathOrContainerName, string fileName);
    Task<List<string>> GetAllFilesAsync(string? pathOrContainerName = null);
}