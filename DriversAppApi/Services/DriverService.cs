using DriversAppApi.Configurations;
using DriversAppApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DriversAppApi.Services
{
    public class DriverService
    {
        private readonly IMongoCollection<Driver> _driverCollection;
        private readonly IMongoCollection<Team> _teamCollection;

        public DriverService(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDB = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            _driverCollection = mongoDB.GetCollection<Driver>(databaseSettings.Value.CollectionName);
            _teamCollection = mongoDB.GetCollection<Team>("Teams");
        }

        public async Task<List<Driver>> GetAsync() => await _driverCollection.Find(_ => true).ToListAsync();

        public async Task<Driver> GetAsync(string id) => await _driverCollection.Find(x => x.Id == id)
            .FirstOrDefaultAsync(); 

        public async Task CreateAsync(Driver driver) => await _driverCollection.InsertOneAsync(driver);

        public async Task UpdateAsync(Driver driver) 
        {
            await _driverCollection
                .ReplaceOneAsync(x => x.Id == driver.Id, driver);
        }

        public async Task DeleteAsync(string id) => await _driverCollection
            .DeleteOneAsync(x => x.Id == id);

        public async Task<List<DriverWithTeam>> GetDriverWithTeamName(string driverId) 
        {
            var result = _driverCollection.Aggregate()
                .Lookup<Driver, Team, DriverWithTeam>(
                    _teamCollection,
                    x => x.TeamId,
                    x => x.Id,
                    x => x.Team
                )
                .ToList();

            return result;
        }
    }
}