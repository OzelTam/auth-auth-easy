using AuthAuthEasyLib.Interfaces;
using AuthAuthEasyLib.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T> 
    {
        public void AddRole(string userId, string role, bool caseInsensitive = false, TimeSpan? roleExpiration = null)
        {
            if (!QueryUsers().Any(u => u._Id == userId))
                throw new KeyNotFoundException("User not found");

            if (QueryUsers(u => u._Id == userId).Any(u => u.Roles.Any(r => r.RoleName == role)))
                throw new DuplicateNameException("Role already assigned");

            if (caseInsensitive)
                if (QueryUsers(u => u._Id == userId).Any(u => u.Roles.Any(r => r.RoleName.ToLower() == role.ToLower())))
                    throw new DuplicateNameException("Role already assigned");

            UpdateUser(userId, (u) => u.Roles.Add(new Role(role, roleExpiration == null? null : DateTime.Now + roleExpiration)));
        }
        public void AddRole(string userId, string[] roles, bool caseInsensitive = false, TimeSpan? roleExpiration = null)
        {
            foreach (var role in roles)
            {
                AddRole(userId, role, caseInsensitive, roleExpiration );
            }
        }


        public async Task AddRoleAsync(string userId, string role, bool caseInsensitive = false, TimeSpan? roleExpiration = null)
        {
            var users = await QueryUsersAsync();
            if (!users.Any(u => u._Id == userId))
                throw new KeyNotFoundException("User not found");

            if (users.Where(u => u._Id == userId).Any(u => u.Roles.Any(r => r.RoleName == role)))
                throw new DuplicateNameException("Role already assigned");

            if (caseInsensitive)
                if (users.Where(u => u._Id == userId).Any(u => u.Roles.Any(r => r.RoleName.ToLower() == role.ToLower())))
                    throw new DuplicateNameException("Role already assigned");

            await UpdateUserAsync(userId, (u) => u.Roles.Add(new Role(role, roleExpiration == null ? null : DateTime.Now + roleExpiration)));
        }
        public async Task AddRoleAsync(string userId, string[] roles, bool caseInsensitive = false, TimeSpan? roleExpiration = null)
        {
            foreach (var role in roles)
            {
                await AddRoleAsync(userId, role, caseInsensitive, roleExpiration);
            }
        }


        public async Task RemoveRoleAsync(string userId, string role, bool caseInsensitive = false)
        {
            var users = await QueryUsersAsync();
            if (!users.Any(u => u._Id == userId))
                throw new KeyNotFoundException("User not found");

            if (!(users.Where(u => u._Id == userId).Any(u => u.Roles.Any(r => r.RoleName.ToLower() == role.ToLower()))))
                return;

            if (!caseInsensitive)
                if (!(users.Where(u => u._Id == userId).Any(u => u.Roles.Any(r => r.RoleName == role))))
                    return;

            var user = await GetUserByIdAsync(userId);
            if (caseInsensitive)
                user.Roles.RemoveAll(r => r.RoleName.ToLower() == role.ToLower());
            else
                user.Roles.RemoveAll(r => r.RoleName == role);

            await crudService.ReplaceAsync(user);
        }
        public async Task RemoveRoleAsync(string userId, string[] roles, bool caseInsensitive = false)
        {
            foreach (var role in roles)
            {
                await RemoveRoleAsync(userId, role, caseInsensitive);
            }
        }


        public void RemoveRole(string userId, string role, bool caseInsensitive = false)
        {
            var users = QueryUsers();
            if (!users.Any(u => u._Id == userId))
                throw new KeyNotFoundException("User not found");

            if (!(users.Where(u => u._Id == userId).Any(u => u.Roles.Any(r => r.RoleName.ToLower() == role.ToLower()))))
                return;

            if (!caseInsensitive)
                if (!(users.Where(u => u._Id == userId).Any(u => u.Roles.Any(r => r.RoleName == role))))
                    return;

            var user = GetUserById(userId);
            if (caseInsensitive)
                user.Roles.RemoveAll(r => r.RoleName.ToLower() == role.ToLower());
            else
                user.Roles.RemoveAll(r => r.RoleName == role);

            crudService.Replace(user);
        }
        public void RemoveRole(string userId, string[] roles, bool caseInsensitive = false)
        {
            foreach (var role in roles)
            {
                RemoveRole(userId, role, caseInsensitive);
            }
        }
        public void RemoveExpiredRoles(string userId)
        {
            UpdateUser(userId, user =>
            {
                user = RemoveExpiredUserRoles(user);
            });
        }
        public async Task RemoveExpiredRolesAsync(string userId)
        {
            await UpdateUserAsync(userId, user =>
            {
                user = RemoveExpiredUserRoles(user);
            });
        }
        public T RemoveExpiredUserRoles(T user)
        {
            if (user == null)
                return null;
            user.Roles = ClearExpiredRoles(user.Roles).ToList();
            return user;
        }
        private IList<Role> ClearExpiredRoles(List<Role> roles)
        {
            roles.RemoveAll(role => role.ExpirationDate.HasValue && DateTimeOffset.Compare(role.ExpirationDate.Value, DateTime.Now) <= 0);
            return roles.ToList();
        }
    

    }
}
