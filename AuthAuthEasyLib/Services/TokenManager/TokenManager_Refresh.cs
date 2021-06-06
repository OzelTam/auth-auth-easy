using AuthAuthEasyLib.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AuthAuthEasyLib.Bases;

namespace AuthAuthEasyLib.Services
{
    public partial class TokenManager<T> where T : class, IAuthUser
    {
        public Token RefreshToken(string tokenKey, TimeSpan span)
        {
            var user = GetUserWithTokenKey(tokenKey);
            Token tokenResult = null;
            user.Tokens.ForEach(token =>
            {
                if (token.Key == tokenKey && token.Expiration != null)
                {
                    token.Expiration = DateTime.Now + span;
                    tokenResult = token;
                }
                    
            });
            crudService.Replace(user);
            return tokenResult;
        }

        public async Task<Token> RefreshTokenAsync(string tokenKey, TimeSpan span)
        {
            var user = await GetUserWithTokenKeyAsync(tokenKey);
            Token tokenResult = null;
            user.Tokens.ForEach(token =>
            {
                if (token.Key == tokenKey && token.Expiration != null)
                {
                    token.Expiration = DateTime.Now + span;
                    tokenResult = token;
                }
                    
            });
            crudService.Replace(user);
            return tokenResult;
        }
    }
}
