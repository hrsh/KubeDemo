namespace KubeDemo.Api.Entities
{
    using System;

    public record SkillEntity
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public int YearsOfExperience { get; init; }

        public int Level { get; init; }
    }
}