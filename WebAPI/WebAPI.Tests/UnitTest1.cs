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

public class ImageControllerTests
{
    private readonly Mock<ImageService> _mockImageService;
    private readonly ImageController _controller;

    public ImageControllerTests()
    {
        var mockClient = new Mock<IMongoClient>();
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockBucket = new Mock<IGridFSBucket>();

        // Ensure `DatabaseNamespace` is set (important for GridFSBucket)
        mockDatabase.Setup(db => db.DatabaseNamespace)
                    .Returns(new DatabaseNamespace("TestDatabase"));

        // Ensure `GetDatabase()` returns the mock database
        mockClient.Setup(client => client.GetDatabase(It.IsAny<string>(), null))
                  .Returns(mockDatabase.Object);

        // Mock `_bucket` behavior in ImageService
        mockDatabase.Setup(db => db.GetCollection<BsonDocument>(It.IsAny<string>(), null))
                    .Returns(Mock.Of<IMongoCollection<BsonDocument>>());
        _mockImageService = new Mock<ImageService>(mockDatabase.Object);
        _controller = new ImageController(_mockImageService.Object);
    }

    [Fact]
    public async Task UploadImage_ReturnsOk_WithFileId()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = "Test file content";
        var fileName = "test.jpg";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns("image/jpeg");

        _mockImageService
            .Setup(s => s.UploadImageAsync(It.IsAny<Stream>(), fileName, "image/jpeg", "Sample description"))
            .ReturnsAsync(ObjectId.GenerateNewId());

        // Act
        var result = await _controller.UploadImage(fileMock.Object, "Sample description");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("FileId", okResult.Value.ToString());
    }

    //     [Fact]
    //     public async Task DownloadImage_ReturnsFile_WhenImageExists()
    //     {
    //         // Arrange
    //         var imageId = ObjectId.GenerateNewId().ToString();
    //         var imageData = Encoding.UTF8.GetBytes("Sample image data");

    //         _mockImageService.Setup(s => s.DownloadImageAsync(It.IsAny<ObjectId>()))
    //                           .ReturnsAsync(imageData);

    //         // Act
    //         var result = await _controller.DownloadImage(imageId);

    //         // Assert
    //         var fileResult = Assert.IsType<FileContentResult>(result);
    //         Assert.Equal("image/jpeg", fileResult.ContentType);
    //     }

    //     [Fact]
    //     public async Task ListImages_ReturnsOk_WithImageList()
    //     {
    //         // Arrange
    //         var files = new[] { new { Id = "1", Name = "Image1", Description = "Sample 1" } };
    //         _mockImageService
    //         .Setup(service => service.ListFilesAsync())
    //         .ReturnsAsync(new List<FileInfoDto> {
    //             new FileInfoDto { Id = "1", Name = "Test.jpg", Description = "Sample image" }
    //         });

    //         // Act
    //         var result = await _controller.ListImages();

    //         // Assert
    //         var okResult = Assert.IsType<OkObjectResult>(result);
    //         Assert.Equal(files, okResult.Value);
    //     }

    //     [Fact]
    //     public async Task DeleteImage_ReturnsOk_WhenFileDeleted()
    //     {
    //         // Arrange
    //         var imageId = ObjectId.GenerateNewId().ToString();
    //         _mockImageService
    //         .Setup(service => service.DeleteFileAsync(It.IsAny<string>()))
    //         .ReturnsAsync(true);
    //         // Act
    //         var result = await _controller.DeleteImage(imageId);

    //         // Assert
    //         var okResult = Assert.IsType<OkObjectResult>(result);
    //         Assert.Contains("File deleted successfully", okResult.Value.ToString());
    //     }

    //     [Fact]
    //     public async Task DeleteImage_ReturnsNotFound_WhenFileNotFound()
    //     {
    //         // Arrange
    //         var imageId = ObjectId.GenerateNewId().ToString();
    //         _mockImageService
    //             .Setup(service => service.DeleteFileAsync(It.IsAny<string>()))
    //             .ThrowsAsync((Exception)new GridFSFileNotFoundException("File not found."));

    //         // Act
    //         var result = await _controller.DeleteImage(imageId);

    //         // Assert
    //         Assert.IsType<NotFoundObjectResult>(result);
    //     }
}