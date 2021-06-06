using AuthAuthEasyLib.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Data.Entity;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T>
    {
        private readonly ICrudService<T> crudService;
        private readonly CrudType crudType;
        internal readonly IDatabase cache;
        private enum CrudType
        {
            Sql,
            NoSql
        }
        public TokenManager<T> TokenManager;
        private void InitializeSubServices()
        {
            this.TokenManager = new TokenManager<T>(crudService, this);
        }
        public AuthService(MongoCrudServiceConfig serviceConfig, IDatabase useCache = null)
        {
            this.crudType = CrudType.NoSql;
            this.crudService = new MongoCrudService<T>(serviceConfig);
            InitializeSubServices();
            this.cache = useCache;
        } // Uses MongoCrud
        public AuthService(string mongoConnectionString, string dbName, string collectionName, IDatabase useCache = null)
        {
            this.cache = useCache;
            this.crudType = CrudType.NoSql;
            this.crudService = new MongoCrudService<T>(mongoConnectionString, dbName, collectionName);
            InitializeSubServices();
        } // Uses MongoCrud
        public AuthService(IMongoCollection<T> mongoCollection, IDatabase useCache = null)
        {
            this.cache = useCache;
            this.crudType = CrudType.NoSql;
            this.crudService = new MongoCrudService<T>(mongoCollection);
            InitializeSubServices();
        } // User MongoCrud
        public AuthService(DbContext context, DbSet<T> usersSet, IDatabase useCache = null)
        {
            this.cache = useCache;
            this.crudType = CrudType.Sql;
            this.crudService = new EFCrudService<T>(context, usersSet);
            InitializeSubServices();
        } // Uses EF6 Sql
    }
}
