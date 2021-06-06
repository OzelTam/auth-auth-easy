using AuthAuthEasyLib.Bases;
using AuthAuthEasyLib.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="password"></param>
        /// <param name="tokenExpiration"></param>
        /// <returns>Returns <see cref="Tokens.AuthToken"/> with default expiration of 7 hours</returns>
        public async Task<(T, Token)> LoginAsync(Expression<Func<T, bool>> expression, string password, TimeSpan? tokenExpiration = null)
        {
            


            var userFound = (await crudService.FindAsync(expression)).FirstOrDefault();
            if (userFound == null)
            {
                throw new UnauthorizedAccessException("User Not Found.");
            }
            else if (BCrypt.Net.BCrypt.Verify(password, userFound.Password, true))
            {
                var authToken = Common.TokenBuilder.GenerateAuthToken(userFound, tokenExpiration==null?TimeSpan.FromHours(7):tokenExpiration);
                await TokenManager.ClearExpiredTokensAsync(userFound._Id);
                await TokenManager.AddTokenAsync(userFound, authToken);
                await CacheSession(userFound, authToken);
                RemoveExpiredRoles(userFound._Id); // Removes Expired Roles
                return (userFound, authToken);
            }
            else
            {
                throw new UnauthorizedAccessException("Wrong password.");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="password"></param>
        /// <param name="tokenExpiration"></param>
        /// <returns>Returns <see cref="Tokens.AuthToken"/> with default expiration of 7 hours</returns>
        public (T, Token) Login(Expression<Func<T, bool>> expression, string password, TimeSpan? tokenExpiration = null)
        {
            var userFound = crudService.Find(expression).FirstOrDefault();
            if (userFound == null)
            {
                throw new UnauthorizedAccessException("Username not exists.");
            }
            else if (BCrypt.Net.BCrypt.Verify(password, userFound.Password, true))
            {
                var authToken = Common.TokenBuilder.GenerateAuthToken(userFound, tokenExpiration ?? TimeSpan.FromDays(7));
                TokenManager.ClearExpiredTokens(userFound._Id);
                TokenManager.AddToken(userFound, authToken);
                CacheSession(userFound, authToken).Wait(TimeSpan.FromSeconds(6));
                RemoveExpiredRoles(userFound._Id); // Removes Expired Roles
                return (userFound, authToken);
            }
            else
            {
                throw new UnauthorizedAccessException("Wrong password.");
            }
        }

    }
}
