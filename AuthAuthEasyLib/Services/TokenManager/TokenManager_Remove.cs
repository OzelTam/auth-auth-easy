using AuthAuthEasyLib.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class TokenManager<T> where T : class, IAuthUser
    {
        public async Task ClearExpiredTokensAsync(string userId)
        {
            var foundUser = (await crudService.FindAsync(z => z._Id == userId))
                .FirstOrDefault();
            var ExpiredTokens = foundUser.Tokens
                .RemoveAll(token =>
                {
                    if (token.Expiration == null)
                        return false;
                    return DateTime.Compare(token.Expiration.Value, DateTime.Now) <= 0;
                }
               );

            await crudService.ReplaceAsync(foundUser);
        }
        public void ClearExpiredTokens(string userId)
        {

            var foundUser = (crudService.Find(z => z._Id == userId))
                .FirstOrDefault();
            var ExpiredTokens = foundUser.Tokens
                .RemoveAll(token => {
                    if (token.Expiration == null)
                        return false;
                    return DateTime.Compare(token.Expiration.Value, DateTime.Now) <= 0;
                });

            crudService.Replace(foundUser);
        }
        public void RemoveToken(string tokenKey, int tokenCode = -1)
        {
            T user = GetUserWithTokenKey(tokenKey, tokenCode);

            user.Tokens.RemoveAll(t => t.Key == tokenKey);

            crudService.Replace(user);

        }
        public async Task RemoveTokenAsync(string tokenKey, int tokenCode = -1)
        {
            T user = await GetUserWithTokenKeyAsync(tokenKey, tokenCode);

            user.Tokens.RemoveAll(t => t.Key == tokenKey);

            await crudService.ReplaceAsync(user);

        }
    }
}
