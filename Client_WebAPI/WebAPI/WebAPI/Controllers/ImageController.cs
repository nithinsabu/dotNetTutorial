using WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using Xunit.Abstractions;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ImageController : ControllerBase
    {
        private readonly ImageService _imageService;
        // private readonly ILogger<ImageController> _logger;
        // private readonly ITestOutputHelper _output;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public ImageController(ImageService imageService, IConfiguration config)
        {
            _imageService = imageService;
            _config = config;
            _httpClient = new HttpClient();
            // _logger = logger;
            // _output = output;
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
            stream.Position = 0;

            //using yolo to detect
            StreamContent sc = new StreamContent(stream);
            MultipartFormDataContent mpfdc = new MultipartFormDataContent();
            mpfdc.Add(sc, "file", file.FileName);
            string responseBody = "";
            try
            {
                using HttpResponseMessage response = await _httpClient.PostAsync(_config.GetConnectionString("fastAPI") + "/upload", mpfdc);
                responseBody = response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : "";
            }
            catch
            {
                responseBody = "";
            }
            // Console.WriteLine(responseBody);
            // dynamic returnObject = JObject.Parse(responseBody);
            // Console.WriteLine(returnObject["person 1"]);
            // return Ok(returnObject);
            return Content(responseBody, "application/json");
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
                return Ok("File deleted successfully.");
            }
            catch (FormatException)
            {
                return BadRequest("Invalid file ID format.");
            }
            catch (GridFSFileNotFoundException)
            {
                return NotFound("File not found.");
            }
            catch (Exception ex)
            {
                // return NotFound("File not found.");
                // _output.WriteLine($"Exception Type: {ex.GetType().FullName}");;
                return StatusCode(500, "An error occurred while deleting the file.");
            }
        }
    }
}
