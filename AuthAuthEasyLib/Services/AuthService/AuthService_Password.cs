using AuthAuthEasyLib.Bases;
using AuthAuthEasyLib.Common;
using AuthAuthEasyLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T>
    {
        public async Task<Token> AddPaswordResetRequestAsync(Expression<Func<T, bool>> getUserExpression, TimeSpan span)
        {
            var user = (await crudService.FindAsync(getUserExpression)).FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var token = TokenBuilder.GeneratePasswordResetToken(user, span);
            await TokenManager.AddTokenAsync(user, token);

            return token;
        }
        public Token AddPaswordResetRequest(Expression<Func<T, bool>> getUserExpression, TimeSpan span)
        {
            var user = crudService.Find(getUserExpression).FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var token = TokenBuilder.GeneratePasswordResetToken(user, span);
            TokenManager.AddToken(user, token);

            return token;
        }

        public async Task ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            await ChangePasswordAsync(user => user._Id == userId, oldPassword, newPassword);
        }
        public async Task ChangePasswordAsync(Expression<Func<T, bool>> expression, string oldPassword, string newPassword)
        {
            var user = (await crudService.FindAsync(expression)).FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException("User Not found.");

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password, true))
                throw new UnauthorizedAccessException("Wrong password.");

            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
            crudService.Replace(user);
        }


        public void ChangePassword(string userId, string oldPassword, string newPassword)
        {
            ChangePassword(user => user._Id == userId, oldPassword, newPassword);
        }
        public void ChangePassword(Expression<Func<T, bool>> expression, string oldPassword, string newPassword)
        {
            var user = crudService.Find(expression).FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException("User Not found.");

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password, true))
                throw new UnauthorizedAccessException("Wrong password.");

            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
            crudService.Replace(user);
        }


        public async Task ResetPasswordAsync(string tokenKey, string newPassword)
        {
            var user = await TokenManager.GetUserWithTokenKeyAsync(tokenKey, 3);
            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
            crudService.Replace(user);
            await TokenManager.RemoveTokenAsync(tokenKey);
        }
        public void ResetPassword(string tokenKey, string newPassword)
        {
            var user = TokenManager.GetUserWithTokenKey(tokenKey, 3);
            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
            crudService.Replace(user);
            TokenManager.RemoveToken(tokenKey);
        }
    }
}
