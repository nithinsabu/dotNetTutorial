using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System.Threading.Tasks;
using WebApplication1.Models;
namespace WebApplication1.Services
{
    public class ImageService
    {
        // private readonly IMongoCollection<User> _users;
        // private readonly IMongoCollection<NameCountry> _nameCountry;
        private readonly GridFSBucket _bucket;
        public ImageService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("UserIp");
            _bucket = new GridFSBucket(database);
        }

    public async Task<ObjectId> UploadImageAsync(Stream fileStream, string fileName, string contentType, string description)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument 
                { 
                    { "contentType", contentType }, 
                    { "description", description } 
                }
            };
            return await _bucket.UploadFromStreamAsync(fileName, fileStream, options);
        }

    public async Task<byte[]> DownloadImageAsync(ObjectId fileId)
    {
        using var stream = new MemoryStream();
        await _bucket.DownloadToStreamAsync(fileId, stream);
        return stream.ToArray();
    }

    public async Task<List<(string Id, string Name, string Description)>> ListFilesAsync()
    {
        var filter = Builders<GridFSFileInfo>.Filter.Empty;
        using var cursor = await _bucket.FindAsync(filter);
        var files = await cursor.ToListAsync();

        return files.Select(f => 
            (f.Id.ToString(), f.Filename, 
            f.Metadata.Contains("description") ? f.Metadata["description"].AsString : "No description")).ToList();
    }
        
    }
}