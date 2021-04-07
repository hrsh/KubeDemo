using KubeDemo.Api.Dtos;
using KubeDemo.Api.Entities;
using KubeDemo.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KubeDemo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController : ControllerBase
    {
        private readonly IMongoRepository _repository;

        public SkillController(IMongoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<SkillEntity> GetSkills()
        {
            var t = _repository.GetSkills();
            return t;
        }

        [HttpGet("{id}")]
        public async Task<SkillEntity> GetSkill(Guid id, CancellationToken ct) =>
            await _repository.GetSkillAsync(id, ct);

        [HttpPost]
        public async Task<IActionResult> CreateSkill(
            [FromBody] SkillAddDto model,
            CancellationToken ct)
        {
            var entity = new SkillEntity
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                YearsOfExperience = model.YearsOfExperience,
                Level = model.Level
            };
            await _repository.Create(entity, ct);
            return Ok();
        }
    }
}