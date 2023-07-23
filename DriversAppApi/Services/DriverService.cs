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
                // .Project<DriverWithTeam>(Builders<DriverWithTeam>.Projection
                //     .Exclude(driverWithTeam => driverWithTeam.Id))
                .ToList();

            return result;
        }

        public async Task<IList<Driver>> GetAllDrivers()
        {
            var filterDefinition = Builders<Driver>.Filter.Empty;
            var allDrivers = _driverCollection.Find(filterDefinition).ToList();

            return allDrivers;
        }

        public async Task<IList<Driver>> GetAllDriversWithEQ(string driverName)
        {
            var filterDefinition = Builders<Driver>.Filter.Eq(x => x.DriverName, driverName);
            var allDrivers = _driverCollection.Find(filterDefinition).ToList();

            return allDrivers;
        }

        public async Task<IList<Driver>> GetAllDriversWithGTE(int driverNumber)
        {
            var filterDefinition = Builders<Driver>.Filter.Gte(x => x.Number, driverNumber);
            var allDrivers = _driverCollection.Find(filterDefinition).ToList();

            return allDrivers;
        }

        public async Task<IList<Driver>> GetAllDriversWithIN(int[] numbers)
        {
            // Busca todos os pilotos cujo o número esteja dentro dos valores especificados
            // pelo array informado no argumento do método
            var filterDefinition = Builders<Driver>.Filter.In(x => x.Number, numbers);
            var allDrivers = _driverCollection.Find(filterDefinition).ToList();

            return allDrivers;
        }

        public async Task<IList<Driver>> GetAllDriversWithCombinedFilters()
        {
            var filterDefinition = Builders<Driver>.Filter.Gt(x => x.Number, 10) &
                Builders<Driver>.Filter.Lt(x => x.Number, 50);

            var allDrivers = _driverCollection.Find(filterDefinition).ToList();

            return allDrivers;
        }

        public async Task UpsertCommand()
        {
            var filterDefinition = Builders<Driver>.Filter.Eq(x => x.Number, 10);
            var updateDefinition = Builders<Driver>.Update
                .Set(x => x.DriverName, "Teste")
                .Set(x => x.Number, 10);

            // IsUpsert: Caso o registro já exista, será atualizado, senão será criado um novo documento
            _driverCollection.UpdateOne(filterDefinition, updateDefinition, new UpdateOptions { IsUpsert = true });
        }

        public Task<Driver> FindOneAndUpdate()
        {
            var findOneAndUpdateOptions = new FindOneAndUpdateOptions<Driver> { ReturnDocument = ReturnDocument.After };
            var filterDefinition = Builders<Driver>.Filter.Eq(x => x.DriverName, "teste");
            var updateDefinition = Builders<Driver>.Update
                .Set(x => x.DriverName, "Teste update");

            var driverAfterUpdate = _driverCollection.FindOneAndUpdate<Driver>(filterDefinition, updateDefinition, findOneAndUpdateOptions);

            return Task.FromResult(driverAfterUpdate);
        }

        public Task<long> CountDocuments()
        {
            var filterDefinition = Builders<Driver>.Filter.Eq(x => x.DriverName, "teste");

            var driversCount = _driverCollection.CountDocuments(filterDefinition);

            return Task.FromResult(driversCount);
        }

        public Task<List<Driver>> GetDriversWithLimit()
        {
            var filterDefinition = Builders<Driver>.Filter.Gt(x => x.Number, 10) &
                Builders<Driver>.Filter.Lt(x => x.Number, 50);

            var allDrivers = _driverCollection.Find(filterDefinition)
                .Limit(2)
                .ToList();

            return Task.FromResult(allDrivers);
        }

        public Task<List<Driver>> GetDriversWithSkip()
        {
            var filterDefinition = Builders<Driver>.Filter.Gt(x => x.Number, 10) &
                Builders<Driver>.Filter.Lt(x => x.Number, 50);

            var allDrivers = _driverCollection.Find(filterDefinition)
                .Skip(2)
                .ToList();

            return Task.FromResult(allDrivers);
        }

        public async Task<Driver> GetDriverById(string driverId)
        {
            var filterDefinition = Builders<Driver>.Filter.Eq(x => x.Id, driverId);
            var driver = await _driverCollection.Find(filterDefinition).FirstOrDefaultAsync();

            return driver;
        }

        public async Task CreateIndex()
        {
            var indexKeys = Builders<Driver>.IndexKeys;
            var indexList = new List<CreateIndexModel<Driver>>
            {
                new CreateIndexModel<Driver>(indexKeys.Ascending(a => a.DriverName), new CreateIndexOptions { Unique = true })
            };

            await _driverCollection.Indexes.CreateManyAsync(indexList);
        }

        public async Task GetDriversWithBatchSize()
        {
            var filterDefinition = Builders<Driver>.Filter.Empty;
            using var cursor = await _driverCollection.FindAsync(filterDefinition, new FindOptions<Driver> { BatchSize = 100});
            List<Driver> productList;
            while(await cursor.MoveNextAsync()) //a cada iteração serão lidos 100 registros
            {
                productList = cursor.Current.ToList(); // preenche a lista para fazer algo futuramente
            }
        }
    }
}