using AuthAuthEasyLib.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public class MongoCrudService<T>: ICrudService<T> where T:IAuthUser
    {
        private IMongoClient mongoClient;
        private IMongoDatabase database;
        private IMongoCollection<T> collection;

        public MongoCrudService(MongoCrudServiceConfig config)
        {
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


            database = mongoClient.GetDatabase(config.DatabaseName);
            collection = database.GetCollection<T>(config.CollectionName);

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
            return result.ToEnumerable() ;
        }


        public T Replace(T user)
        {
          return  collection.FindOneAndReplace<T>(x => x._Id == user._Id, user);
        }

        public async Task<T> ReplaceAsync(T user)
        {
            return await collection.FindOneAndReplaceAsync<T>(x => x._Id == user._Id, user);
        }

        public T DeleteOne(Expression<Func<T, bool>> expression) {
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


    }
}
