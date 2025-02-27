using MongoDB.Driver;
using System.Threading.Tasks;
using WebApplication1.Models;
namespace WebApplication1.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<NameCountry> _nameCountry;
        public UserService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("UserIp");
            _users = database.GetCollection<User>("Users");
            _nameCountry = database.GetCollection<NameCountry>("NameCountry");
        }
        public async Task SaveNameCountryAsync(string name, string country)
        {
            var nameCountry = new NameCountry { Name = name, Country = country };
            await _nameCountry.InsertOneAsync(nameCountry);
        }
        public async Task<List<NameCountry>> GetAllNameCountryAsync()
        {
            return await _nameCountry.Find(_ => true).ToListAsync();
        }
        public async Task<NameCountry?> GetByIdAsync(string id)
        {
            return await _nameCountry.Find(nc => nc.Id == id).FirstOrDefaultAsync();
        }
        public async Task SaveUserAsync(string ip)
        {
            var user = new User { IPAddress = ip };
            await _users.InsertOneAsync(user);
        }
        public async Task<User> GetUserByIPAsync(string ip)
        {
            return await _users.Find(user => user.IPAddress == ip).FirstOrDefaultAsync();
        }
    }
}