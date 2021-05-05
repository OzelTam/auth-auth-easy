using MongoDB.Driver;
using System.Security.Authentication;

namespace AuthAuthEasyLib.Services
{
    public class MongoCrudServiceConfig
    {
        internal enum SetMethod
        {
            ConnectionString,
            ClientSettings,
            Local


        }

        public MongoCrudServiceConfig(string connectionString, string databaseName, string collectionName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
            CollectionName = collectionName;
            ConfigType = SetMethod.ConnectionString;
        }
        public MongoCrudServiceConfig(MongoClientSettings mongoClientSettings, string databaseName, string collectionName)
        {
            MongoSettings = mongoClientSettings;
            DatabaseName = databaseName;
            CollectionName = collectionName;
            ConfigType = SetMethod.ClientSettings;
        }

        public MongoCrudServiceConfig(string databaseName, string collectionName)
        {
            DatabaseName = databaseName;
            CollectionName = collectionName;
            ConfigType = SetMethod.Local;
        }

        internal SetMethod ConfigType { get; }

        public MongoClientSettings MongoSettings { get;}
        public string ConnectionString { get;}
        public string DatabaseName { get ;}
        public string CollectionName { get;}


    }
}
