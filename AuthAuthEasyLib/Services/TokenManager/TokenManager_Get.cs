using AuthAuthEasyLib.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class TokenManager<T> where T : IAuthUser
    {
        public string GetUserIdWithTokenKey(string tokenKey, int tokenCode = -1)
        {
            var userId = tokenCode == -1
                ? crudService.FindQueriable().Where(u => u.Tokens.Any(t => t.Key == tokenKey)).Select(u => u._Id).FirstOrDefault()
                : crudService.FindQueriable().Where(u => u.Tokens.Any(t => t.Key == tokenKey)).Select(u => u._Id).FirstOrDefault();
            return userId;
        }
        public Task<string> GetUserIdWithTokenKeyAsync(string tokenKey, int tokenCode = -1)
        {
            return Task.Run(() => GetUserIdWithTokenKey(tokenKey, tokenCode));
        }
        public async Task<T> GetUserWithTokenKeyAsync(string tokenKey, int tokenCode = -1)
        {
            T user = tokenCode <= -1
                ? (await crudService.FindAsync(u => u.Tokens.Any(t => t.Key == tokenKey))).FirstOrDefault()
                : (await crudService.FindAsync(u => u.Tokens.Any(t => t.Key == tokenKey && t.TokenCode == tokenCode))).FirstOrDefault();

            return user == null ? throw new KeyNotFoundException("Token not found.") : user;
        }
        public T GetUserWithTokenKey(string tokenKey, int tokenCode = -1)
        {
            T user = tokenCode == -1
                ? crudService.Find(u => u.Tokens.Any(t => t.Key == tokenKey)).FirstOrDefault()
                : crudService.Find(u => u.Tokens.Any(t => t.Key == tokenKey && t.TokenCode == tokenCode)).FirstOrDefault();
            return user == null ? throw new KeyNotFoundException("Token not found.") : user;
        }
    }
}
