using System.Collections.Generic;
using System.Threading.Tasks;
using KubeDemo.Api.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KubeDemo.Api.Repositories
{
    public class MongoRepository
    {
        private readonly ILogger<MongoRepository> _logger;
        private readonly MongoDbOptions _options;

        public MongoRepository(
            ILogger<MongoRepository> logger,
            IOptions<MongoDbOptions> options
        )
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<List<SkillEntity>> GetSkillsAsync()
        {
            return new List<SkillEntity>();
        }
    }
}