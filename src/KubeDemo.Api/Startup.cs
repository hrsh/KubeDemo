using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using KubeDemo.Api.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace KubeDemo.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        private const string _HealthCheckReadyState = "ready";

        private const string _HealthCheckLiveState = "live";

        public Startup(IConfiguration configuration) =>
            _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            services.Configure<MongoDbOptions>(
                _configuration.GetSection(nameof(MongoDbOptions)));

            var options = _configuration
                .GetSection(nameof(MongoDbOptions))
                .Get<MongoDbOptions>();

            services.AddSingleton<IMongoClient>(s =>
            {
                return new MongoClient(options.ConnectionString);
            });

            services.AddSingleton<IMongoRepository, MongoRepository>();

            services.AddControllers();
            services.AddHealthChecks()
                .AddMongoDb(options.ConnectionString,
                    name: "mongodb",
                    timeout: TimeSpan.FromSeconds(5),
                    tags: new[] { _HealthCheckReadyState });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks($"/hc/{_HealthCheckReadyState}", new HealthCheckOptions
                {
                    Predicate = (chk) => chk.Tags.Contains(_HealthCheckReadyState),
                    ResponseWriter = async (context, report) =>
                    {
                        var result = JsonSerializer.Serialize(new
                        {
                            status = $"Status: {report.Status}",
                            duration = $"Total duration: {report.TotalDuration}",
                            checks = report.Entries.Select(e => new
                            {
                                name = e.Key,
                                status = e.Value.Status,
                                description = e.Value.Description,
                                duration = e.Value.Duration,
                                exception = e.Value.Exception is not null ? e.Value.Exception.Message : "nil"
                            })
                        });
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                });
                endpoints.MapHealthChecks($"/hc/{_HealthCheckLiveState}", new HealthCheckOptions
                {
                    Predicate = (_) => false
                });
            });
        }
    }
}
