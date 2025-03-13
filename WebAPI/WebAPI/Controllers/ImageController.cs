using WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System.Runtime.InteropServices;
namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ImageController : ControllerBase
    {
        private readonly ImageService _imageService;
        private readonly ILogger<ImageController> _logger;

        public ImageController(ImageService imageService, ILogger<ImageController> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }
        // [HttpGet]
        // public string Sample(){
        //     return "Image";
        // }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromForm] string description)
        {
            if (file == null || string.IsNullOrWhiteSpace(description))
            {
                return BadRequest("File and description are required.");
            }

            using var stream = file.OpenReadStream();
            var fileId = await _imageService.UploadImageAsync(stream, file.FileName, file.ContentType, description);
            return Ok(new { FileId = fileId.ToString() });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadImage(string id)
        {
            Console.WriteLine(id);

            if (!ObjectId.TryParse(id, out var fileId))
                return BadRequest("Invalid ObjectId format.");

            var imageData = await _imageService.DownloadImageAsync(fileId);
            if (imageData == null || imageData.Length == 0)
                return NotFound("Image not found or empty.");
            return File(imageData, "image/jpeg"); 
        }

        [HttpGet]
        public async Task<IActionResult> ListImages()
        {
            var files = await _imageService.ListFilesAsync();
            // Console.WriteLine(string.Join("\n", files.Select(f => $"ID: {f.Id}, Name: {f.Name}, Description: {f.Description}")));

            return Ok(files);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(string id)
        {
            try
            {
                await _imageService.DeleteFileAsync(id);
                return Ok("File deleted successfully." );
            }
            catch (FormatException)
            {
                return BadRequest("Invalid file ID format." );
            }
            catch (GridFSFileNotFoundException)
            {
                return NotFound("File not found.");
            }
            catch (Exception)
            {
                // return NotFound("File not found.");
                return StatusCode(500, "An error occurred while deleting the file." );
            }
        }
    }
}
