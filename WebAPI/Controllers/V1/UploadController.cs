using Asp.Versioning;
using Business.Utilities.Storage;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    public class UploadController(IStorageService storageService) : ControllerBase
    {
        [MapToApiVersion("1.0")]
        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile(string pathOrContainer, IFormFile file)
        {
            var result = await storageService.UploadFileAsync(pathOrContainer, file);
            return Ok(result);
        }

        [MapToApiVersion("1.0")]
        [HttpPost("upload-files")]
        public async Task<IActionResult> UploadFiles(string pathOrContainer, IFormFileCollection files)
        {
            var result = await storageService.UploadFilesAsync(pathOrContainer, files);
            return Ok(result);
        }

        [MapToApiVersion("1.0")]
        [HttpDelete("delete-file")]
        public async Task<IActionResult> DeleteFile(string pathOrContainer, string filename)
        {
            await storageService.DeleteAsync(pathOrContainer, filename);
            return Ok();
        }

        [MapToApiVersion("1.0")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllFiles([FromQuery] string? pathOrContainer = null)
        {
            var response = await storageService.GetAllFilesAsync(pathOrContainer);
            return Ok(response);
        }
    }
}
