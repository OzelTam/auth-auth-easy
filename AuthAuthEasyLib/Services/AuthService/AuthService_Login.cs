using AuthAuthEasyLib.Bases;
using AuthAuthEasyLib.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T> where T : IAuthUser
    {
        public async Task<(T, Token)> LoginAsync(Expression<Func<T, bool>> expression, string password)
        {
            return await LoginAsync(expression, password, new TimeSpan(7, 0, 0, 0));
        }
        public async Task<(T, Token)> LoginAsync(Expression<Func<T, bool>> expression, string password, TimeSpan lifeTimespan)
        {
            var userFound = (await crudService.FindAsync(expression)).FirstOrDefault();

            if (userFound == null)
            {
                throw new UnauthorizedAccessException("Username not exists.");
            }
            else if (BCrypt.Net.BCrypt.Verify(password, userFound.Password, true))
            {
                var authToken = Common.AuthBuilders.GenerateAuthToken(userFound, lifeTimespan);
                await TokenManager.ClearExpiredTokensAsync(userFound._Id);
                await TokenManager.AddTokenAsync(userFound, authToken);
                return (userFound, authToken);
            }
            else
            {
                throw new UnauthorizedAccessException("Wrong password.");
            }
        }
        public (T, Token) Login(Expression<Func<T, bool>> expression, string password)
        {
            return Login(expression, password, new TimeSpan(7, 0, 0, 0));
        }
        public (T, Token) Login(Expression<Func<T, bool>> expression, string password, TimeSpan lifeTimespan)
        {
            var userFound = crudService.Find(expression).FirstOrDefault();
            if (userFound == null)
            {
                throw new UnauthorizedAccessException("Username not exists.");
            }
            else if (BCrypt.Net.BCrypt.Verify(password, userFound.Password, true))
            {
                var authToken = Common.AuthBuilders.GenerateAuthToken(userFound, lifeTimespan);
                TokenManager.ClearExpiredTokens(userFound._Id);
                TokenManager.AddToken(userFound, authToken);
                return (userFound, authToken);
            }
            else
            {
                throw new UnauthorizedAccessException("Wrong password.");
            }
        }



        #region Login With
        public async Task<(T, Token)> LoginWithEmailAsync(string email, string password)
        {
            return await LoginAsync(u => u.Email == email, password);
        }
        public (T, Token) LoginWithEmail(string email, string password)
        {
            return Login(u => u.Email == email, password);
        }
        public async Task<(T, Token)> LoginWithUsernameAsync(string username, string password)
        {
            return await LoginAsync(u => u.Username == username, password);
        }
        public (T, Token) LoginWithUsername(string username, string password)
        {
            return Login(u => u.Username == username, password);
        }
        #endregion
    }
}
