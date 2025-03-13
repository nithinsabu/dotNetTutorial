using WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImageController(ImageService imageService)
        {
            _imageService = imageService;
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
            return File(imageData, "image/jpeg"); // Change content type as needed
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
                return Ok(new { Message = "File deleted successfully." });
            }
            catch (FormatException)
            {
                return BadRequest(new { Error = "Invalid file ID format." });
            }
            catch (GridFSFileNotFoundException)
            {
                return NotFound(new { Error = "File not found." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "An error occurred while deleting the file." });
            }
        }
    }
}
