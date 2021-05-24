using AuthAuthEasyLib.Common;
using AuthAuthEasyLib.Interfaces;
using System;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T> where T : IAuthUser
    {
        public async Task RegisterAsync(T newUser, RegisterOptions options = null)
        {
            TimeSpan span = new TimeSpan(7, 0, 0, 0, 0);
            if (options?.VerificationSpan != null) { span = options.VerificationSpan; }

            if (options == null && !String.IsNullOrWhiteSpace(newUser.Username))
                options = new RegisterOptions() { RequireUniqueUsernames = true };
            else if (options == null)
                options = new RegisterOptions();


            await AuthBuilders.ValidateNewUserAsync<T>(newUser, crudService, options);



            newUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newUser.Password);
            var VerificationToken = Common.AuthBuilders.GenerateVerificationToken(newUser, span); // Creates Verifcation Token
            newUser.Tokens.Add(VerificationToken); // Add Token to Users Tokens

            await crudService.AddAsync(newUser);

        }
        public void Register(T newUser, RegisterOptions options = null)
        {
            TimeSpan span = new TimeSpan(7, 0, 0, 0, 0);
            if (options?.VerificationSpan != null) { span = options.VerificationSpan; }

            if (options == null && !String.IsNullOrWhiteSpace(newUser.Username))
                options = new RegisterOptions() { RequireUniqueUsernames = true };
            else if (options == null)
                options = new RegisterOptions();


            AuthBuilders.ValidateNewUser<T>(newUser, crudService, options);



            newUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newUser.Password);
            var VerificationToken = Common.AuthBuilders.GenerateVerificationToken(newUser, span); // Creates Verifcation Token
            newUser.Tokens.Add(VerificationToken); // Add Token to Users Tokens

            crudService.Add(newUser);
        }
    }
}
