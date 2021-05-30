using AuthAuthEasyLib.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    internal class MongoCrudService<T> : ICrudService<T> where T : IAuthUser
    {

        private IMongoCollection<T> collection;
        private IQueryable<T> queryableCollection => collection.AsQueryable();


        public MongoCrudService(string mongoConnectionString, string dbName, string collectionName)
        {
            var mongoClient = new MongoClient(mongoConnectionString);
            var database  = mongoClient.GetDatabase(dbName);
            collection = database.GetCollection<T>(collectionName);

            var uniqueProps = typeof(T).GetProperties()
               .Where(prop => Attribute.IsDefined(prop, typeof(Attributes.UniqueAttribute)));

            foreach (var uniqueProp in uniqueProps)
            {
                Task.Run(async () =>
                {
                    var options = new CreateIndexOptions() { Unique = true };
                    var field = new StringFieldDefinition<T>(uniqueProp.Name);
                    var indexDefinition = new IndexKeysDefinitionBuilder<T>().Ascending(field);
                    await collection.Indexes.CreateOneAsync(indexDefinition, options);
                });
            }
        }
        public MongoCrudService(MongoCrudServiceConfig config)
        {
            IMongoClient mongoClient = null;
            switch (config.ConfigType)
            {
                case MongoCrudServiceConfig.SetMethod.ConnectionString:
                    mongoClient = new MongoClient(config.ConnectionString);
                    break;

                case MongoCrudServiceConfig.SetMethod.ClientSettings:
                    mongoClient = new MongoClient(config.MongoSettings);
                    break;

                case MongoCrudServiceConfig.SetMethod.Local:
                    mongoClient = new MongoClient();
                    break;
            }
            var database = mongoClient.GetDatabase(config.DatabaseName);
            collection = database.GetCollection<T>(config.CollectionName);

            var uniqueProps = typeof(T).GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(Attributes.UniqueAttribute)));

            foreach (var uniqueProp in uniqueProps)
            {
                Task.Run(async () =>
                {
                    var options = new CreateIndexOptions() { Unique = true };
                    var field = new StringFieldDefinition<T>(uniqueProp.Name);
                    var indexDefinition = new IndexKeysDefinitionBuilder<T>().Ascending(field);
                    await collection.Indexes.CreateOneAsync(indexDefinition, options);
                });
            }

        }
        public MongoCrudService(IMongoClient mongoClient, string dbName, string collectionName)
        { 
            var client = mongoClient;
           var database = client.GetDatabase(dbName);
            this.collection = database.GetCollection<T>(collectionName);
        }
        public MongoCrudService(IMongoCollection<T> mongoCollection)
        {
            collection = mongoCollection;
        }


        public void Add(T user)
        {
            collection.InsertOne(user);
        }
        public void Add(IEnumerable<T> users)
        {
            collection.InsertMany(users);
        }
        public async Task AddAsync(T user)
        {

            await collection.InsertOneAsync(user);
        }
        public async Task AddAsync(IEnumerable<T> users)
        {
            await collection.InsertManyAsync(users);
        }
        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            var result = collection.Find(expression);
            return result.ToEnumerable();
        }
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            var result = await collection.FindAsync(expression);
            return result.ToEnumerable();
        }
        public T Replace(T user)
        {
            return collection.FindOneAndReplace<T>(x => x._Id == user._Id, user);
        }
        public async Task<T> ReplaceAsync(T user)
        {
            return await collection.FindOneAndReplaceAsync<T>(x => x._Id == user._Id, user);
        }
        public T DeleteOne(Expression<Func<T, bool>> expression)
        {
            return collection.FindOneAndDelete(expression);
        }
        public async Task<T> DeleteOneAsync(Expression<Func<T, bool>> expression)
        {
            return await collection.FindOneAndDeleteAsync(expression);
        }
        public long Delete(Expression<Func<T, bool>> expression)
        {
            return collection.DeleteMany(expression).DeletedCount;
        }
        public async Task<long> DeleteAsync(Expression<Func<T, bool>> expression)
        {
            return (await collection.DeleteManyAsync(expression)).DeletedCount;
        }
        public IQueryable<T> FindQueriable(Expression<Func<T, bool>> expression)
        {
            return queryableCollection.Where(expression);
        }
        public IQueryable<T> FindQueriable()
        {
            return queryableCollection;
        }
    }
}
