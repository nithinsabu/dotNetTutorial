using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Threading.Tasks;
using WebApplication1.Services;
using MongoDB.Bson;

namespace WebApplication1.Controllers;

public class ImageController : Controller
{
    private readonly ILogger<ImageController> _logger;
    private readonly ImageService _imageService;
    public ImageController(ILogger<ImageController> logger)
    {
        _logger = logger;
        _imageService = new ImageService();
    }

    [HttpGet("image")]
    public async Task<IActionResult> Index()
    {
        var images = await _imageService.ListFilesAsync();
        return View(images);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    ///  Returns a page to upload image
    /// </returns>
    [HttpGet("image/upload")]
    public IActionResult Upload()
    {
        
        return View();
    }

    [HttpGet("image/{id}")]
    public async Task<IActionResult> GetImage(string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId))
            return BadRequest("Invalid ObjectId.");

        var imageBytes = await _imageService.DownloadImageAsync(objectId);
        if (imageBytes == null) return NotFound();
        return File(imageBytes, "image/jpeg");
    }

    [HttpPost("/image/upload")]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromForm] string description)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = file.OpenReadStream();
        var fileId = await _imageService.UploadImageAsync(stream, file.FileName, file.ContentType, description);
        return View("~/Views/Image/Upload.cshtml");
    }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
