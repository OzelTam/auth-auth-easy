using AuthAuthEasyLib.Bases;
using AuthAuthEasyLib.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T>
    {
        public async Task CacheSession(T user, Token token, TimeSpan? unusedExpiration = null)
        {
            if (cache == null)
                return;
            

            var sessionCacheObject = new Models.SessionCacheObject() { 
                Roles = user.Roles, 
                Token = token, 
                UserId = user._Id, 
                AbsoluteExpiration = token.Expiration ?? DateTime.Now.Add(TimeSpan.FromMinutes(30)), 
                UnusedExpiration = unusedExpiration ?? TimeSpan.FromMinutes(2)
            };
            var sessionJsonData = JsonConvert.SerializeObject(sessionCacheObject);
            var key = new RedisKey(token.Key);
            await cache.StringSetAsync(key, sessionJsonData, expiry: sessionCacheObject.UnusedExpiration);
           
        } 
        public async Task<SessionCacheObject> GetSessionCache(string tokenKey)
        {
            if (cache == null)
                return null;

            var jsonData = await cache.StringGetAsync(new RedisKey(tokenKey));
           
            var result = jsonData.HasValue ? JsonConvert.DeserializeObject<SessionCacheObject>(jsonData.ToString()) : null;
            if(result!= null)
            {
                var willExpire = DateTime.Now.Add(result.UnusedExpiration);
                bool validSlide = result.AbsoluteExpiration == null 
                    ? true
                    : DateTime.Compare(willExpire, result.AbsoluteExpiration.Value) <= 0;

                if(validSlide)
                    await cache.KeyExpireAsync(new RedisKey(tokenKey), willExpire, CommandFlags.FireAndForget);
            }


            return result;
        }
        public async Task<SessionCacheObject> UpdateSessionCache(string tokenKey, Action<SessionCacheObject> action)
        {
            if (cache == null)
                return null;

            var sessionObject = await  GetSessionCache(tokenKey);
            action(sessionObject);
            var jsonData = JsonConvert.SerializeObject(sessionObject);
            await cache.StringSetAsync(new RedisKey(tokenKey), new RedisValue(jsonData), sessionObject.UnusedExpiration);

            return sessionObject;

        }



    }
}
