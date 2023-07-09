using DriversAppApi.Configurations;
using DriversAppApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DriversAppApi.Services
{
    public class DriverService
    {
        private readonly IMongoCollection<Driver> _driverCollection;

        public DriverService(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDB = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            _driverCollection = mongoDB.GetCollection<Driver>(databaseSettings.Value.CollectionName);
        }

        public async Task<List<Driver>> GetAsync() => await _driverCollection.Find(_ => true).ToListAsync();
    }
}