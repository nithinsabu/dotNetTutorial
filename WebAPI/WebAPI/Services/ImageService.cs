using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System.Threading.Tasks;
namespace WebAPI.Services;
public class FileInfoDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

    public class ImageService
    {
        // private readonly IMongoCollection<User> _users;
        // private readonly IMongoCollection<NameCountry> _nameCountry;
        private readonly GridFSBucket _bucket;
        private readonly IMongoDatabase _database;

        public ImageService(IMongoDatabase database)
        {
            _database = database;
            _bucket = new GridFSBucket(database);
        }

        /// <summary>
        /// Takes Stream and uploads file to gridFS.
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <param name="description"></param>
        /// <returns></returns>
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

        public async Task<byte[]?> DownloadImageAsync(ObjectId fileId)
        {
           try
            {
                using var stream = new MemoryStream();
                await _bucket.DownloadToStreamAsync(fileId, stream);
                return stream.ToArray();
            }
            catch (GridFSFileNotFoundException)
            {
                return null;
            }
        }

        public async Task<List<FileInfoDto>> ListFilesAsync()
        {
            var filter = Builders<GridFSFileInfo>.Filter.Empty;
            using var cursor = await _bucket.FindAsync(filter);
            var files = await cursor.ToListAsync();

            return files.Select(f => new FileInfoDto
            {
                Id = f.Id.ToString(),
                Name = f.Filename,
                Description = f.Metadata.Contains("description")
                    ? f.Metadata["description"].AsString
                    : "No description"
            }).ToList();
        }

        public async Task<bool> DeleteFileAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new FormatException("Invalid ObjectId format.");

            try
            {
                var filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(x => x.Id, objectId);
                var cursor = await _bucket.FindAsync(filter);
                var fileExists = await cursor.AnyAsync();
                if (!fileExists)
                    throw new GridFSFileNotFoundException("File not found.");

                await _bucket.DeleteAsync(objectId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while deleting the file.", ex);
            }
        }
    }
