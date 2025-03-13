using WebAPI.Controllers;
using WebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Mongo2Go;
using Microsoft.Extensions.Logging;

public class ImageControllerTests: IDisposable
{
    private readonly MongoDbRunner _runner;
    private readonly IMongoDatabase _database;
    private readonly ImageService _imageService;
    private readonly ImageController _controller;
    private readonly ILogger<ImageController> _logger;
    public ImageControllerTests()
    {

        _runner = MongoDbRunner.Start(); // Starts a lightweight MongoDB instance
        var client = new MongoClient(_runner.ConnectionString);
        _database = client.GetDatabase("TestDatabase");

        _imageService = new ImageService(_database);  // Use the real ImageService

        _logger = LoggerFactory
            .Create(builder => builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information))
            .CreateLogger<ImageController>();
        _controller = new ImageController(_imageService, _logger);
    }

    [Fact]
    public async Task UploadImage_ReturnsOk_WithFileId()
    {
        var fileMock = new Mock<IFormFile>();
        var content = "Test file content";
        var fileName = "test.jpg";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns("image/jpeg");

        var result = await _controller.UploadImage(fileMock.Object, "Sample description");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task UploadImage_ReturnsBadRequest_WhenFileIsNull()
    {
        var result = await _controller.UploadImage(null, "Sample description");

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("File and description are required.", badRequestResult.Value);
    }

    [Fact]
    public async Task UploadImage_ReturnsBadRequest_WhenDescriptionIsNull()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
        fileMock.Setup(f => f.FileName).Returns("test.jpg");
        fileMock.Setup(f => f.ContentType).Returns("image/jpeg");

        var result = await _controller.UploadImage(fileMock.Object, null);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("File and description are required.", badRequestResult.Value);
    }

    [Fact]
    public async Task DownloadImage_ReturnsFile_WhenImageExists()
    {
        // Arrange
        var sampleData = Encoding.UTF8.GetBytes("Sample Image Data");
        var fileId = await _imageService.UploadImageAsync(new MemoryStream(sampleData), "sample.jpg", "image/jpeg", "Test Description");

        // Act
        var result = await _controller.DownloadImage(fileId.ToString());

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("image/jpeg", fileResult.ContentType);
        Assert.Equal(sampleData, fileResult.FileContents);
    }

    [Fact]
    public async Task DownloadImage_ReturnsBadRequest_WhenInvalidObjectId()
    {
        // Act
        var result = await _controller.DownloadImage("invalid_object_id");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid ObjectId format.", badRequestResult.Value);
    }

    [Fact]
    public async Task DownloadImage_ReturnsNotFound_WhenImageDoesNotExist()
    {
        // Act
        var result = await _controller.DownloadImage(ObjectId.GenerateNewId().ToString());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

     [Fact]
    public async Task ListImages_ReturnsEmptyList_WhenNoImagesExist()
    {
        var result = await _controller.ListImages();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var files = Assert.IsType<List<FileInfoDto>>(okResult.Value);
        Assert.Empty(files);
    }

    [Fact]
    public async Task ListImages_ReturnsAllImages_WhenImagesExist()
    {
        // Arrange
        await _imageService.UploadImageAsync(new MemoryStream(new byte[] { 1, 2, 3 }), "image1.jpg", "image/jpeg", "Desc 1");
        await _imageService.UploadImageAsync(new MemoryStream(new byte[] { 4, 5, 6 }), "image2.jpg", "image/jpeg", "Desc 2");

        // Act
        var result = await _controller.ListImages();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var files = Assert.IsType<List<FileInfoDto>>(okResult.Value);

        // Assert
        Assert.Equal(2, files.Count);
        Assert.Contains(files, f => f.Name == "image1.jpg");
        Assert.Contains(files, f => f.Name == "image2.jpg");
    }

    [Fact]
    public async Task DeleteImage_ReturnsOk_WhenFileDeletedSuccessfully()
    {
        var fileId = await _imageService.UploadImageAsync(new MemoryStream(Encoding.UTF8.GetBytes("Test content")), "test.jpg", "image/jpeg", "Sample description");

        var result = await _controller.DeleteImage(fileId.ToString());

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("File deleted successfully.", okResult.Value);
    }

    [Fact]
    public async Task DeleteImage_ReturnsBadRequest_WhenInvalidIdFormat()
    {
        var result = await _controller.DeleteImage("invalid_id");

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid file ID format.", badRequestResult.Value);
    }

    // [Fact]
    // public async Task DeleteImage_ReturnsNotFound_WhenFileDoesNotExist()
    // {
    //     var fakeId = ObjectId.GenerateNewId().ToString();
    //     var result = await _controller.DeleteImage(fakeId);
    //     var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    //     Assert.Equal("File not found.", notFoundResult.Value);
    // }

    [Fact]
    public async Task DeleteImage_ReturnsServerError_OnUnexpectedError()
    {

        var result = await _controller.DeleteImage(ObjectId.GenerateNewId().ToString());

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while deleting the file.", statusCodeResult.Value);
    }

     public void Dispose()
    {
        _runner.Dispose(); 
    }
    
}