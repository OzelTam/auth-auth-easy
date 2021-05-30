using AuthAuthEasyLib.Interfaces;
using MongoDB.Driver;
using System.Data.Entity;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T>
    {
        private readonly ICrudService<T> crudService;
        private readonly CrudType crudType;
        private enum CrudType
        {
            Sql,
            NoSql
        }
        public TokenManager<T> TokenManager;
        private void InitializeSubServices()
        {
            this.TokenManager = new TokenManager<T>(crudService);
        }
        public AuthService(MongoCrudServiceConfig serviceConfig)
        {
            this.crudType = CrudType.NoSql;
            this.crudService = new MongoCrudService<T>(serviceConfig);
            InitializeSubServices();
        } // Uses MongoCrud
        public AuthService(string mongoConnectionString, string dbName, string collectionName)
        {
            this.crudType = CrudType.NoSql;
            this.crudService = new MongoCrudService<T>(mongoConnectionString, dbName, collectionName);
            InitializeSubServices();
        } // Uses MongoCrud
        public AuthService(IMongoCollection<T> mongoCollection)
        {
            this.crudType = CrudType.NoSql;
            this.crudService = new MongoCrudService<T>(mongoCollection);
            InitializeSubServices();
        } // User MongoCrud
        public AuthService(DbContext context, DbSet<T> usersSet)
        {
            this.crudType = CrudType.Sql;
            this.crudService = new EFCrudService<T>(context, usersSet);
            InitializeSubServices();
        } // Uses EF6 Sql
    }
}
