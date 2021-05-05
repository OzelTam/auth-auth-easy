using AuthAuthEasyLib.Bases;
using AuthAuthEasyLib.Common;
using AuthAuthEasyLib.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public class AuthService<T> where T : IAuthUser
    {
        private readonly ICrudService<T> crudService;
        private TokenManagerService<T> tokenManager;

        private TimeSpan defaultTimeSpan = new TimeSpan(7, 0, 0, 0, 0); // DETE THIS AFTER CREATING AUTH OPTIONS
        private void InitializeSubServices()
        {
            this.tokenManager = new TokenManagerService<T>(crudService);
        }

        public AuthService(ICrudService<T> crudService)
        {
            this.crudService = crudService;
            InitializeSubServices();
        }

        public AuthService(MongoCrudServiceConfig serviceConfig)
        {
            this.crudService = new MongoCrudService<T>(serviceConfig);
            InitializeSubServices();
        }


        public  async Task<T> GetUserByIdAsync(string userId)
        {
            return (await crudService.FindAsync(usr => usr._Id == userId)).FirstOrDefault();
        }
        public T GetUserById(string userId)
        {
            return crudService.Find(usr => usr._Id == userId).FirstOrDefault();
        }

        #region Root Register Methods
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

        #endregion

        #region Root Login Methods
        public async Task<(IAuthUser, Token)> LoginAsync(Expression<Func<T, bool>> expression, string password)
        {
            var userFound = (await crudService.FindAsync(expression)).FirstOrDefault();

            if (userFound == null)
            {
                throw new UnauthorizedAccessException("Username not exists.");
            }
            else if (BCrypt.Net.BCrypt.Verify(password, userFound.Password, true))
            {
                var authToken = Common.AuthBuilders.GenerateAuthToken(userFound, defaultTimeSpan);
                await tokenManager.ClearExpiredTokensAsync(userFound);
                await tokenManager.AddTokenAsync(userFound, authToken);
                return (userFound, authToken);
            }
            else
            {
                throw new UnauthorizedAccessException("Wrong password.");
            }
        }
        public (T, Token) Login(Expression<Func<T, bool>> expression, string password)
        {
            var userFound = crudService.Find(expression).FirstOrDefault();
            if (userFound == null)
            {
                throw new UnauthorizedAccessException("Username not exists.");
            }
            else if (BCrypt.Net.BCrypt.Verify(password, userFound.Password,true))
            {
                var authToken = Common.AuthBuilders.GenerateAuthToken(userFound, defaultTimeSpan);
                tokenManager.ClearExpiredTokens(userFound);
                tokenManager.AddToken(userFound, authToken);
                return (userFound, authToken);
            }
            else
            {
                throw new UnauthorizedAccessException("Wrong password.");
            }
        }
        #endregion

        #region Login With Methods
        public async Task<(IAuthUser, Token)> LoginWithEmailAsync(string email, string password)
        {
            return await LoginAsync(u => u.Email == email, password);
        }
        public (IAuthUser, Token) LoginWithEmail(string email, string password)
        {
            return Login(u => u.Email == email, password);
        }
        public async Task<(IAuthUser, Token)> LoginWithUsernameAsync(string username, string password)
        {
            return await LoginAsync(u => u.Username == username, password);
        }
        public (IAuthUser, Token) LoginWithUsername(string username, string password)
        {
            return Login(u => u.Username == username, password);
        }
        #endregion

        #region Verify User Methods

        public  async Task VerifyUserAsync(string tokenKey) 
        {
            var user = await tokenManager.GetUserWithTokenKeyAsync(tokenKey);

            var verificationToken = user.Tokens.Where(tkn =>
            tkn.TokenCode == 2 &&
            tkn.Key == tokenKey).FirstOrDefault();

            if (verificationToken == null)
                throw new Exception("Invalid verification key.");
            else if (DateTime.Compare(verificationToken.Expiration, DateTime.Now) < 0)
            {
                throw new Exception("Verification token is expired");
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
            var user = tokenManager.GetUserWithTokenKey(tokenKey);

            var verificationToken = user.Tokens.Where(tkn =>
            tkn.TokenCode == 2 &&
            tkn.Key == tokenKey).FirstOrDefault();

            if (verificationToken == null)
                throw new Exception("Invalid verification key.");
            else if (DateTime.Compare(verificationToken.Expiration, DateTime.Now) < 0)
            {
                throw new Exception("Verification token is expired");
            }
            else
            {
                user.Tokens.Remove(verificationToken);
                user.IsVerified = true;
                crudService.Replace(user);
            }

        }


        #endregion

        #region Verification Token Methods
        public async Task<Token> GetOrCreateVerificationTokenAsync(string userId, TimeSpan tokenSpan)
        {
            var user = (await crudService.FindAsync(u => u._Id == userId)).FirstOrDefault();
            if (user == null)
                throw new Exception("User not found");
            else if (user.IsVerified)
                throw new Exception("User already verified.");

            tokenManager.ClearExpiredTokens(user);

            var userVToken = user.Tokens.Where(t => t.TokenCode == 2).FirstOrDefault();
            if (userVToken == null)
            {
                userVToken = Common.AuthBuilders.GenerateVerificationToken(user, tokenSpan);
                await tokenManager.AddTokenAsync(user, userVToken);
            }

            return userVToken;
        }
        public Token GetOrCreateVerificationToken(string userId, TimeSpan tokenSpan)
        {
            var user = crudService.Find(u => u._Id == userId).FirstOrDefault();
            if (user == null)
                throw new Exception("User not found");
            else if (user.IsVerified)
                throw new Exception("User already verified.");

            tokenManager.ClearExpiredTokens(user);

            var userVToken = user.Tokens.Where(t => t.TokenCode == 2).FirstOrDefault();
            if (userVToken == null)
            {
                userVToken = Common.AuthBuilders.GenerateVerificationToken(user, tokenSpan);
                tokenManager.AddToken(user, userVToken);
            }

            return userVToken;
        } 
        #endregion

        #region LogOut Methods
        public void LogOut(string key)
        {
            tokenManager.RemoveToken(key, 1);
        }
        public async Task LogOutAsync(string key)
        {
           await  tokenManager.RemoveTokenAsync(key, 1);
        }
        #endregion

        #region IsAuthorized Methods
        public async Task<(bool, Exception)> IsAuthorizedAsync(string tokenKey)
        {
            var user = await tokenManager.GetUserWithTokenKeyAsync(tokenKey, 1);


            var userToken = user.Tokens.Where(t => t.Key == tokenKey).FirstOrDefault();
            if (userToken == null)
                return (false, new UnauthorizedAccessException("Invalid token key."));

            if (DateTime.Compare(userToken.Expiration, DateTime.Now) < 0)
            {
                await tokenManager.ClearExpiredTokensAsync(user);
                return (false, new UnauthorizedAccessException("Token has been expired."));
            }

            return (true, null);

        }
        public (bool, Exception) IsAuthorized(string tokenKey)
        {
            var user = tokenManager.GetUserWithTokenKey(tokenKey, 1);

            var userToken = user.Tokens.Where(t => t.Key == tokenKey).FirstOrDefault();
            if (userToken == null)
                return (false, new UnauthorizedAccessException("Invalid token key."));

            if (DateTime.Compare(userToken.Expiration, DateTime.Now) < 0)
            {
                tokenManager.ClearExpiredTokens(user);
                return (false, new UnauthorizedAccessException("Token has been expired."));
            }

            return (true, null);

        }
        #endregion

        #region User Update Info Methods

        public async Task UpdateUserAsync(string userId, Action<T> updateMethod)
        {
            var user = await GetUserByIdAsync(userId);
            updateMethod(user);
            await crudService.ReplaceAsync(user);
        }
        public void UpdateUser(string userId, Action<T> updateMethod)
        {
            var user = GetUserById(userId);
            updateMethod(user);
            crudService.Replace(user);
        }

        public async Task<Token> AddPaswordResetRequestAsync(Expression<Func<T, bool>> getUserExpression, TimeSpan span)
        {
            var user = (await crudService.FindAsync(getUserExpression)).FirstOrDefault();
            if (user == null)
                throw new Exception("User not found.");

            var token = AuthBuilders.GeneratePasswordResetToken(user, span);
            await tokenManager.AddTokenAsync(user, token);

            return token;
        }
        public Token AddPaswordResetRequest(Expression<Func<T, bool>> getUserExpression, TimeSpan span)
        {
            var user = crudService.Find(getUserExpression).FirstOrDefault();
            if (user == null)
                throw new Exception("User not found.");

            var token = AuthBuilders.GeneratePasswordResetToken(user, span);
            tokenManager.AddToken(user, token);

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
                throw new Exception("User Not found.");

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password, true))
                throw new UnauthorizedAccessException("Wrong password.");

            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
            crudService.Replace(user);
        }
       
        public void ChangePassword(string userId, string oldPassword, string newPassword)
        {
            ChangePassword(user => user._Id == userId, oldPassword, newPassword);
        }
        public void ChangePassword(Expression<Func<T,bool>> expression, string oldPassword, string newPassword)
        {
            var user = crudService.Find(expression).FirstOrDefault();
            if (user == null)
                throw new Exception("User Not found.");

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password, true))
                throw new UnauthorizedAccessException("Wrong password.");

            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
            crudService.Replace(user);
        }
       
        
        public async Task ResetPasswordAsync(string tokenKey, string newPassword)
        {
            var user = await tokenManager.GetUserWithTokenKeyAsync(tokenKey, 3);
            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
            crudService.Replace(user);
            await tokenManager.RemoveTokenAsync(tokenKey);
        }
        public void ResetPassword(string tokenKey, string newPassword)
        {
            var user = tokenManager.GetUserWithTokenKey(tokenKey, 3);
            user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
            crudService.Replace(user);
            tokenManager.RemoveToken(tokenKey);
        }

        #endregion



    }


}
