using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KubeDemo.Api.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using MongoDB.Bson;

namespace KubeDemo.Api.Repositories
{
    public interface IMongoRepository
    {
        Task Create(SkillEntity model, CancellationToken ct = default);

        Task<SkillEntity> GetSkillAsync(Guid id, CancellationToken ct = default);

        Task<List<SkillEntity>> GetSkillsAsync(CancellationToken ct = default);
    }

    public class MongoRepository : IMongoRepository
    {
        private readonly ILogger<MongoRepository> _logger;
        private readonly MongoDbOptions _options;
        private readonly IMongoCollection<SkillEntity> _skillCollection;
        private const string _database = "skill";
        private const string _collection = "skill";

        public MongoRepository(
            ILogger<MongoRepository> logger,
            IOptions<MongoDbOptions> options,
            IMongoClient mongoClient
        )
        {
            _logger = logger;
            _options = options.Value;
            _logger.LogCritical($"mongo connection string: {options.Value.ConnectionString}");

            var database = mongoClient.GetDatabase(_database);
            _skillCollection = database.GetCollection<SkillEntity>(_collection);
        }

        public async Task<List<SkillEntity>> GetSkillsAsync(CancellationToken ct = default) =>
            await _skillCollection
                .Find(new BsonDocument())
                .ToListAsync(ct);

        public async Task<SkillEntity> GetSkillAsync(
            Guid id,
            CancellationToken ct = default) =>
            await _skillCollection.Find(a => a.Id == id).FirstOrDefaultAsync(ct);

        public async Task Create(
            SkillEntity model,
            CancellationToken ct = default)
            => await _skillCollection
                .InsertOneAsync(
                        model,
                        new InsertOneOptions
                        {
                            BypassDocumentValidation = true
                        },
                        ct);
    }
}