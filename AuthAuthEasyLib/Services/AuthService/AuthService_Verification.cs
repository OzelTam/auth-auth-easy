using AuthAuthEasyLib.Bases;
using AuthAuthEasyLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T>
    {
        public async Task VerifyUserAsync(string tokenKey)
        {
            var user = await TokenManager.GetUserWithTokenKeyAsync(tokenKey);

            var verificationToken = user.Tokens.Where(tkn =>
            tkn.TokenCode == 2 &&
            tkn.Key == tokenKey).FirstOrDefault();

            if (verificationToken == null)
                throw new InvalidOperationException("Invalid verification key.");
            else if (DateTime.Compare(verificationToken.Expiration, DateTime.Now) < 0)
            {
                throw new UnauthorizedAccessException("Verification token is expired");
            }
            else
            {
                user.Tokens.Remove(verificationToken);
                user.IsVerified = true;
                await crudService.ReplaceAsync(user);
            }

        }
        public void VerifyUser(string tokenKey)
        {
            var user = TokenManager.GetUserWithTokenKey(tokenKey);

            var verificationToken = user.Tokens.Where(tkn =>
            tkn.TokenCode == 2 &&
            tkn.Key == tokenKey).FirstOrDefault();

            if (verificationToken == null)
                throw new InvalidOperationException("Invalid verification key.");
            else if (DateTime.Compare(verificationToken.Expiration, DateTime.Now) < 0)
            {
                throw new UnauthorizedAccessException("Verification token is expired");
            }
            else
            {
                user.Tokens.Remove(verificationToken);
                user.IsVerified = true;
                crudService.Replace(user);
            }

        }
        public async Task<Token> GetOrCreateVerificationTokenAsync(string userId, TimeSpan tokenSpan)
        {
            var user = (await crudService.FindAsync(u => u._Id == userId)).FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException("User not found");
            else if (user.IsVerified)
                throw new InvalidOperationException("User already verified.");

            TokenManager.ClearExpiredTokens(user._Id);

            var userVToken = user.Tokens.Where(t => t.TokenCode == 2).FirstOrDefault();
            if (userVToken == null)
            {
                userVToken = Common.TokenBuilder.GenerateVerificationToken(user, tokenSpan);
                await TokenManager.AddTokenAsync(user, userVToken);
            }

            return userVToken;
        }
        public Token GetOrCreateVerificationToken(string userId, TimeSpan tokenSpan)
        {
            var user = crudService.Find(u => u._Id == userId).FirstOrDefault();
            if (user == null)
                throw new KeyNotFoundException("User not found");
            else if (user.IsVerified)
                throw new InvalidOperationException("User already verified.");

            TokenManager.ClearExpiredTokens(user._Id);

            var userVToken = user.Tokens.Where(t => t.TokenCode == 2).FirstOrDefault();
            if (userVToken == null)
            {
                userVToken = Common.TokenBuilder.GenerateVerificationToken(user, tokenSpan);
                TokenManager.AddToken(user, userVToken);
            }

            return userVToken;
        }

    }
}
