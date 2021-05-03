namespace AuthAuthEasyLib.Services
{
    public class MongoCrudServiceConfig
    {
        public MongoCrudServiceConfig(string connectionString, string databaseName, string collectionName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }

        public string ConnectionString { get ; set ; }
        public string DatabaseName { get ; set ; }
        public string CollectionName { get; set; }


    }
}
