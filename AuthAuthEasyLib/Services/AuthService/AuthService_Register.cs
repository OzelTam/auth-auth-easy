using AuthAuthEasyLib.Common;
using AuthAuthEasyLib.Interfaces;
using System;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T> where T : IAuthUser
    {

        /// <summary>
        /// Registers new user with the default verification token span of 7 days.
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public async Task RegisterAsync(T newUser)
        {
            await RegisterAsync(newUser, new TimeSpan(7, 0, 0, 0));
        }

        /// <summary>
        /// Registers new user with the default verification token span of 7 days.
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public void Register(T newUser)
        {
            Register(newUser, new TimeSpan(7, 0, 0, 0));
        }

        public async Task RegisterAsync(T newUser, TimeSpan verificationTokenSpan)
        {

            newUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newUser.Password);
            var VerificationToken = TokenBuilder.GenerateVerificationToken(newUser, verificationTokenSpan); // Creates Verifcation Token
            newUser.Tokens.Add(VerificationToken); // Add Token to Users Tokens

            await crudService.AddAsync(newUser);

        }
        public void Register(T newUser, TimeSpan verificationTokenSpan)
        {

            newUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newUser.Password);
            var VerificationToken = TokenBuilder.GenerateVerificationToken(newUser, verificationTokenSpan); // Creates Verifcation Token
            newUser.Tokens.Add(VerificationToken); // Add Token to Users Tokens

            crudService.Add(newUser);
        }
    }
}
