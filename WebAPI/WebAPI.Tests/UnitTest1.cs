using Xunit;
using Moq;
using WebAPI.Services;
using WebAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Tests
{
    public class ImageControllerTests
    {
        private readonly Mock<ImageService> _mockImageService;
        private readonly ImageController _controller;

        public ImageControllerTests()
        {
            _mockImageService = new Mock<ImageService>(mockDependency.Object);
            _controller = new ImageController(_mockImageService.Object);
        }

        // ============ UploadImage Tests ============
        [Fact]
        public async Task UploadImage_ValidInput_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "File content";
            var fileName = "test.jpg";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");

            _mockImageService.Setup(s => s.UploadImageAsync(It.IsAny<Stream>(), fileName, "image/jpeg", "Sample Description"))
                .ReturnsAsync(ObjectId.GenerateNewId());

            // Act
            var result = await _controller.UploadImage(fileMock.Object, "Sample Description");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("FileId", okResult.Value.ToString());
        }

        [Fact]
        public async Task UploadImage_MissingFile_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UploadImage(null, "Sample Description");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // ============ DownloadImage Tests ============
        [Fact]
        public async Task DownloadImage_ValidId_ReturnsFile()
        {
            // Arrange
            var imageData = Encoding.UTF8.GetBytes("FakeImageData");
            _mockImageService.Setup(s => s.DownloadImageAsync(It.IsAny<ObjectId>()))
                             .ReturnsAsync(imageData);

            // Act
            var result = await _controller.DownloadImage(ObjectId.GenerateNewId().ToString());

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("image/jpeg", fileResult.ContentType);
        }

        [Fact]
        public async Task DownloadImage_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.DownloadImage("invalid-id");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // ============ ListImages Tests ============
        [Fact]
        public async Task ListImages_ReturnsFileList()
        {
            // Arrange
            var mockFiles = new List<FileInfoDto>
            {
                new FileInfoDto { Id = "1", Name = "Image1", Description = "Description1" },
                new FileInfoDto { Id = "2", Name = "Image2", Description = "Description2" }
            };

            _mockImageService.Setup(s => s.ListFilesAsync()).ReturnsAsync(mockFiles);

            // Act
            var result = await _controller.ListImages();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFiles = Assert.IsType<List<FileInfoDto>>(okResult.Value);
            Assert.Equal(2, returnedFiles.Count);
        }

        // ============ DeleteImage Tests ============
        [Fact]
        public async Task DeleteImage_ValidId_ReturnsOk()
        {
            // Arrange
            _mockImageService.Setup(s => s.DeleteFileAsync(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteImage(ObjectId.GenerateNewId().ToString());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteImage_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.DeleteImage("invalid-id");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteImage_FileNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockImageService.Setup(s => s.DeleteFileAsync(It.IsAny<string>()))
                .ThrowsAsync(new GridFSFileNotFoundException("File not found."));

            // Act
            var result = await _controller.DeleteImage(ObjectId.GenerateNewId().ToString());

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
