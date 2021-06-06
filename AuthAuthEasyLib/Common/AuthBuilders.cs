using AuthAuthEasyLib.Bases;
using AuthAuthEasyLib.Interfaces;
using AuthAuthEasyLib.Services;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Common
{
    internal static class TokenBuilder
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


        public static Token GenerateAuthToken(IAuthUser authUser, TimeSpan? span = null)
        {
            string userRepresentation = String.IsNullOrWhiteSpace(authUser._Id)
                ? RandomString(15, 60)
                : authUser._Id;

            return new Tokens.AuthToken(GenerateKey(userRepresentation), span);
        }

        public static Token GenerateVerificationToken(IAuthUser authUser, TimeSpan? span = null)
        {
            string userRepresentation = String.IsNullOrWhiteSpace(authUser._Id)
             ? RandomString(15, 60)
             : authUser._Id;
            return new Tokens.VerificationToken(ComputeMD5(GenerateKey(userRepresentation)), span);
        }

        public static Token GeneratePasswordResetToken(IAuthUser authUser, TimeSpan? span = null)
        {
            string userRepresentation = String.IsNullOrWhiteSpace(authUser._Id)
                       ? RandomString(15, 60)
                       : authUser._Id;
            return new Tokens.ResetPasswordToken(GenerateKey(userRepresentation), span);
        }

    }
}
