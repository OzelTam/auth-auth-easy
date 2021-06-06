using AuthAuthEasyLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class TokenManager<T> where T : class, IAuthUser
    {
        public string GetUserIdWithTokenKey(string tokenKey, int tokenCode = -1)
        {
            
            if (authService.cache != null) // Check Cache
            {
                var session = authService.GetSessionCache(tokenKey).Result;
                if(session != null)
                {
                    string uid;

                    if (tokenCode < 0)
                        uid = session.UserId;
                    else
                        uid = session.Token.TokenCode == tokenCode 
                            ?session.UserId
                            : null;

                    if (!String.IsNullOrEmpty(uid))
                        return uid;
                }
            } 
            var userId = tokenCode == -1
                ? crudService.FindQueriable().Where(u => u.Tokens.Any(t => t.Key == tokenKey)).Select(u => u._Id).FirstOrDefault()
                : crudService.FindQueriable().Where(u => u.Tokens.Any(t => t.Key == tokenKey)).Select(u => u._Id).FirstOrDefault();

            if (userId == null)
                throw new UnauthorizedAccessException("Invalid token key.");

            return userId;
        }
        public Task<string> GetUserIdWithTokenKeyAsync(string tokenKey, int tokenCode = -1)
        {
            return Task.Run(() => GetUserIdWithTokenKey(tokenKey, tokenCode));
        }
        public async Task<T> GetUserWithTokenKeyAsync(string tokenKey, int tokenCode = -1)
        {
            var uid = await GetUserIdWithTokenKeyAsync(tokenKey, tokenCode);
            T user = (await crudService.FindAsync(u => u._Id == uid)).FirstOrDefault();
            return user ?? throw new UnauthorizedAccessException("Invalid token key.") ;
        }
        public T GetUserWithTokenKey(string tokenKey, int tokenCode = -1)
        {
            var uid =  GetUserIdWithTokenKey(tokenKey, tokenCode);
            T user = crudService.Find(u => u._Id == uid).FirstOrDefault();
            return user ?? throw new UnauthorizedAccessException("Invalid token key.") ;
        }
    }
}
