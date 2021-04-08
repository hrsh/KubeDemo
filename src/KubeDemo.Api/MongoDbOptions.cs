namespace KubeDemo.Api
{
    public class MongoDbOptions
    {
        public string Host { get; set; }

        public string Port { get; set; }

        public string Password { get; set; }

        public string User { get; set; }

        public string ConnectionString =>
            $"mongodb://{User}:{Password}@{Host}:{Port}";
    }
}