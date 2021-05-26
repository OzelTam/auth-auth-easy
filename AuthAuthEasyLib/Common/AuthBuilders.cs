using AuthAuthEasyLib.Bases;
using AuthAuthEasyLib.Interfaces;
using AuthAuthEasyLib.Services;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Common
{
    internal static class AuthBuilders
    {


        private static Random random = new Random();
        private static string ComputeMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private static string RandomString(int minLength = 10, int maxLength = 50)
        {
            var length = random.Next(minLength, maxLength);
            const string chars = @"qwertyuıopğüasdfghjklşizxcvbnmöçQWERTYUIOPĞÜASDFGHJKLŞİZXCVBNMÖ0123456789-_4'!?+*/\,";
            return new string(Enumerable.Repeat(chars, Math.Abs(length))
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static string GenerateKey(string userRepresentation)
        {
            byte[] randomString = Encoding.UTF8.GetBytes(RandomString());
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] uId = Encoding.ASCII.GetBytes(userRepresentation);
            return Convert.ToBase64String(randomString.Concat(time).Concat(uId).ToArray());
        }

        public static Token GenerateAuthToken(IAuthUser authUser, TimeSpan span)
        {
            string userRepresentation = String.IsNullOrWhiteSpace(authUser._Id)
                ? RandomString(15, 60)
                : authUser._Id;

            return new Tokens.AuthToken(GenerateKey(userRepresentation), span);
        }

        public static Token GenerateVerificationToken(IAuthUser authUser, TimeSpan span)
        {
            string userRepresentation = String.IsNullOrWhiteSpace(authUser._Id)
             ? RandomString(15, 60)
             : authUser._Id;
            return new Tokens.VerificationToken(ComputeMD5(GenerateKey(userRepresentation)), span);
        }

        public static Token GeneratePasswordResetToken(IAuthUser authUser, TimeSpan span)
        {
            string userRepresentation = String.IsNullOrWhiteSpace(authUser._Id)
                       ? RandomString(15, 60)
                       : authUser._Id;
            return new Tokens.ResetPasswordToken(GenerateKey(userRepresentation), span);
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
                throw new InvalidOperationException("Email already in use");
            }

            if (options.RequireUniqueUsernames && relatedUsers.Count(usr => usr.Username == newUser.Username) > 0)
            {
                throw new InvalidOperationException("Username is taken");
            }

            if (options.RequireUniquePhoneNumbers && relatedUsers.Count(usr => usr.PhoneNumber == newUser.PhoneNumber) > 0)
            {
                throw new InvalidOperationException("Phone number is already in use");
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
                throw new InvalidOperationException("Email already in use");
            }

            if (options.RequireUniqueUsernames && relatedUsers.Count(usr => usr.Username == newUser.Username) > 0)
            {
                throw new InvalidOperationException("Username is taken");
            }

            if (options.RequireUniquePhoneNumbers && relatedUsers.Count(usr => usr.PhoneNumber == newUser.PhoneNumber) > 0)
            {
                throw new InvalidOperationException("Phone number is already in use");
            }

            return true;
        }

    }
}
