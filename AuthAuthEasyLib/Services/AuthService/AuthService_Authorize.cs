using AuthAuthEasyLib.Bases;
using AuthAuthEasyLib.Interfaces;
using AuthAuthEasyLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T>
    {

        public async Task<(bool, Exception, T)> GetAuthorityAsync(string authTokenKey)
        {
            var user = await TokenManager.GetUserWithTokenKeyAsync(authTokenKey, 1);
            var userToken = user.Tokens?.FirstOrDefault(t => t.Key == authTokenKey && t.TokenCode == 1);
            if (userToken == null)
                return (false, new UnauthorizedAccessException("Invalid token key."), user);

            if(userToken.Expiration != null)
            {
                if (DateTime.Compare(userToken.Expiration.Value, DateTime.Now) < 0)
                {
                    await TokenManager.ClearExpiredTokensAsync(user._Id);
                    return (false, new UnauthorizedAccessException("Token has been expired."), user);
                }
            }
            

            return (true, null, user);

        }
        public async Task<(bool, Exception, T)> GetAuthorityAsync(string authTokenKey, string requiredRole, bool caseInsensetive = false)
        {
            var (isValid, except, user) = await GetAuthorityAsync(authTokenKey);
            if (!isValid)
                return (false, except, user);
            else
            {
                if (CheckRole(user, requiredRole, caseInsensetive))
                    return (true, except, user);
                else
                    return (false, new UnauthorizedAccessException("Unauthorized access."), user);
            }

        }
        public async Task<(bool, Exception, T)> GetAuthorityAsync(string authTokenKey, string[] requiredRoles,bool caseInsensetive = false)
        {
            var (isValid, except, user) = await GetAuthorityAsync(authTokenKey);
            if (!isValid)
                return (false, except, user);
            else
            {
                if (CheckRole(user, requiredRoles, caseInsensetive))
                    return (true, except, user);
                else
                    return (false, new UnauthorizedAccessException("Unauthorized access."), user);
            }

        }



        public (bool, Exception, T) GetAuthority(string authTokenKey)
        {
            var user = TokenManager.GetUserWithTokenKey(authTokenKey, 1);

            var userToken = user.Tokens.Where(t => t.Key == authTokenKey).FirstOrDefault();
            if (userToken == null)
                return (false, new UnauthorizedAccessException("Invalid token key."), user);

            if(userToken.Expiration != null)
            {
                if (DateTime.Compare(userToken.Expiration.Value, DateTime.Now) < 0)
                {
                    TokenManager.ClearExpiredTokens(user._Id);
                    return (false, new UnauthorizedAccessException("Token has been expired."), user);
                }
            }
            

            return (true, null, user);

        }
        public (bool, Exception, T) GetAuthority(string authTokenKey, string requiredRole, bool caseInsensetive = false)
        {
            var (isValid, except, user) = GetAuthority(authTokenKey);
            if (!isValid)
                return (false, except, user);
            else
            {
                if (CheckRole(user, requiredRole, caseInsensetive))
                    return (true, except, user);
                else
                    return (false, new UnauthorizedAccessException("Unauthorized access."), user);
            }
        }
        public (bool, Exception, T) GetAuthority(string authTokenKey, string[] requiredRoles, bool caseInsensetive = false)
        {
            var (isValid, except, user) = GetAuthority(authTokenKey);
            if (!isValid)
                return (false, except, user);
            else
            {
                if (CheckRole(user, requiredRoles, caseInsensetive))
                    return (true, except, user);
                else
                    return (false, new UnauthorizedAccessException("Unauthorized access."), user);
            }
        }




        private async Task RefreshSessionToken(string authTokenKey, TimeSpan resetExpiration )
        {
                var freshToken = await TokenManager.RefreshTokenAsync(authTokenKey, resetExpiration);
                await UpdateSessionCache(authTokenKey, token =>
                {
                    token.AbsoluteExpiration = freshToken.Expiration;
                });
        }



        public async Task<bool> CheckAuth(string authTokenKey, TimeSpan? resetExpiration = null)
        {
            var session = GetSessionCache(authTokenKey).Result;
            if (session != null)
                return true;

            try
            {

                var uid = TokenManager.GetUserIdWithTokenKey(authTokenKey, 1); // Find User Id
                TokenManager.ClearExpiredTokens(uid); //Clear expired Tokens
                uid = TokenManager.GetUserIdWithTokenKey(authTokenKey, 1); // Find User Id Again (if token expired throws error) 
                var isExists = crudService.FindQueriable().Any(u => u._Id == uid);  // Check if exists

                if (isExists && resetExpiration.HasValue)                           
                    await RefreshSessionToken(authTokenKey, resetExpiration.Value); //Refresh If Exists

                return isExists;
            }
            catch
            {
                return false;
            }

        }
        public async Task<bool> CheckAuth(string authTokenKey, string authorizedRole, bool caseInsensetive = false, TimeSpan? resetExpiration = null)
        {
           var session = GetSessionCache(authTokenKey).Result;
           if (session != null && CheckRole(session?.Roles, authorizedRole))
               return true;


            try
            {
                var uid = TokenManager.GetUserIdWithTokenKey(authTokenKey, 1); // Find User Id
                TokenManager.ClearExpiredTokens(uid); //Clear expired Tokens
                uid = TokenManager.GetUserIdWithTokenKey(authTokenKey, 1); // Find User Id Again (if token expired throws error) 
                var roles = crudService.FindQueriable(u => u._Id == uid)
                    .Select(u => u.Roles).FirstOrDefault();


                var isExists = caseInsensetive 
                    ? roles.Any(r => r.RoleName.ToLower() == authorizedRole.ToLower()) 
                    : roles.Any(r => r.RoleName == authorizedRole);
               

                    if(isExists && resetExpiration.HasValue)
                    await RefreshSessionToken(authTokenKey, resetExpiration.Value); //Refresh If Exists

                return isExists;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> CheckAuth(string authTokenKey, string[] authorizedRoles,bool caseInsensetive = false, TimeSpan? resetExpiration = null)
        {
            var session = GetSessionCache(authTokenKey).Result;
            if (session != null && CheckRole(session?.Roles, authorizedRoles))
                return true;

            foreach (var role in authorizedRoles)
            {
                if (await CheckAuth(authTokenKey, role, caseInsensetive, resetExpiration))
                    return true;
            }

            return false;
        }




        #region CheckRole
        private bool CheckRole(T user, string role, bool caseInsensetive = false)
        {
            if (!caseInsensetive)
                return RemoveExpiredUserRoles(user).Roles.Any(rol => rol.RoleName == role);
            else
                return RemoveExpiredUserRoles(user).Roles.Any(rol => rol.RoleName.ToLower() == role.ToLower());
        }
        private bool CheckRole(T user, string[] roles, bool caseInsensetive = false)
        {
            foreach (var role in roles)
            {

                if (CheckRole(user, role, caseInsensetive))
                    return true;
            }
            return false;
        }
        private bool CheckRole(IEnumerable<Role> roleList,string role, bool caseInsensitive = false)
        {
            roleList = ClearExpiredRoles(roleList.ToList());
            return caseInsensitive
                ? roleList.Any(r => r.RoleName.ToLower() == role.ToLower())
                : roleList.Any(r => r.RoleName == role);
        }
        private bool CheckRole(IEnumerable<Role> roleList, string[] roles, bool caseInsensitive = false)
        {
            foreach (var role in roles)
            {
                if (CheckRole(roleList, role))
                    return true;
            }
            return false;
        }

        #endregion
    }
}
