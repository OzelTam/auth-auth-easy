using AuthAuthEasyLib.Bases;
using AuthAuthEasyLib.Interfaces;
using AuthAuthEasyLib.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Common
{
    internal static class AuthBuilders
    {
        public static Token GenerateAuthToken(IAuthUser authUser, TimeSpan span)
        {
            
            var Key = BCrypt.Net.
                BCrypt.HashPassword(
                authUser.Password + DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), 
                BCrypt.Net.BCrypt.GenerateSalt().ToString(),
                true,
                BCrypt.Net.HashType.SHA512);
            return new Tokens.AuthToken(Key, span);
        }

        public static Token GenerateVerificationToken(IAuthUser authUser, TimeSpan span)
        {

            var Key = BCrypt.Net.
                BCrypt.HashPassword(
                authUser.Password + DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                BCrypt.Net.BCrypt.GenerateSalt().ToString(),
                true,
                BCrypt.Net.HashType.SHA512);
            return new Tokens.VerificationToken(Key, span);
        }

        public static Token GeneratePasswordResetToken(IAuthUser authUser, TimeSpan span)
        {
            var Key = BCrypt.Net.
                BCrypt.HashPassword(
                authUser.Password + DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                BCrypt.Net.BCrypt.GenerateSalt().ToString(),
                true,
                BCrypt.Net.HashType.SHA512);
            return new Tokens.ResetPasswordToken(Key, span);
        }

        public static async Task<bool> ValidateNewUserAsync<T>(T newUser, ICrudService<T> crudService, RegisterOptions options) where T : IAuthUser
        {

            var relatedUsers = (await crudService.FindAsync(u =>
                               u.Email == newUser.Email ||
                               u.PhoneNumber == newUser.PhoneNumber ||
                               u.Username == newUser.Username)).ToList();

            if (relatedUsers.Count() == 0) { return true; } // If No Similar Users Found


            if (options.RequireUniqueEmails && relatedUsers.Count(usr => usr.Email == newUser.Email) > 0)
            {
                throw new Exception("Email already in use");
            }

            if (options.RequireUniqueUsernames && relatedUsers.Count(usr => usr.Username == newUser.Username) > 0)
            {
                throw new Exception("Username is taken");
            }

            if (options.RequireUniquePhoneNumbers && relatedUsers.Count(usr => usr.PhoneNumber == newUser.PhoneNumber) > 0)
            {
                throw new Exception("Phone number is already in use");
            }

            return true;
        }
        public static bool ValidateNewUser<T>(T newUser, ICrudService<T> crudService, RegisterOptions options) where T : IAuthUser
        {

            var relatedUsers = crudService.Find(u =>
                               u.Email == newUser.Email ||
                               u.PhoneNumber == newUser.PhoneNumber ||
                               u.Username == newUser.Username).ToList();

            if (relatedUsers.Count() == 0) { return true; } // If No Similar Users Found


            if (options.RequireUniqueEmails && relatedUsers.Count(usr => usr.Email == newUser.Email) > 0)
            {
                throw new Exception("Email already in use");
            }

            if (options.RequireUniqueUsernames && relatedUsers.Count(usr => usr.Username == newUser.Username) > 0)
            {
                throw new Exception("Username is taken");
            }

            if (options.RequireUniquePhoneNumbers && relatedUsers.Count(usr => usr.PhoneNumber == newUser.PhoneNumber) > 0)
            {
                throw new Exception("Phone number is already in use");
            }

            return true;
        }

    }
}
