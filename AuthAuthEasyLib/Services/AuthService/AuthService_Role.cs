using AuthAuthEasyLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T> 
    {
        public void AddRole(string userId, string role, bool caseInsensitive = false)
        {
            if (!QueryUsers().Any(u => u._Id == userId))
                throw new KeyNotFoundException("User not found");

            if (QueryUsers(u => u._Id == userId).Any(u => u.Roles.Any(r => r == role)))
                throw new DuplicateNameException("Role already assigned");

            if (caseInsensitive)
                if (QueryUsers(u => u._Id == userId).Any(u => u.Roles.Any(r => r.ToLower() == role.ToLower())))
                    throw new DuplicateNameException("Role already assigned");

            UpdateUser(userId, (u) => u.Roles.Add(role));
        }
        public void AddRole(string userId, string[] roles, bool caseInsensitive = false)
        {
            foreach (var role in roles)
            {
                AddRole(userId, role, caseInsensitive);
            }
        }
        public async Task AddRoleAsync(string userId, string role, bool caseInsensitive = false)
        {
            var users = await QueryUsersAsync();
            if (!users.Any(u => u._Id == userId))
                throw new KeyNotFoundException("User not found");

            if (users.Where(u => u._Id == userId).Any(u => u.Roles.Any(r => r == role)))
                throw new DuplicateNameException("Role already assigned");

            if (caseInsensitive)
                if (users.Where(u => u._Id == userId).Any(u => u.Roles.Any(r => r.ToLower() == role.ToLower())))
                    throw new DuplicateNameException("Role already assigned");

            await UpdateUserAsync(userId, (u) => u.Roles.Add(role));
        }
        public async Task AddRoleAsync(string userId, string[] roles, bool caseInsensitive = false)
        {
            foreach (var role in roles)
            {
                await AddRoleAsync(userId, role, caseInsensitive);
            }
        }
        public async Task RemoveRoleAsync(string userId, string role, bool caseInsensitive = false)
        {
            var users = await QueryUsersAsync();
            if (!users.Any(u => u._Id == userId))
                throw new KeyNotFoundException("User not found");

            if (!(users.Where(u => u._Id == userId).Any(u => u.Roles.Any(r => r.ToLower() == role.ToLower()))))
                return;

            if (!caseInsensitive)
                if (!(users.Where(u => u._Id == userId).Any(u => u.Roles.Any(r => r == role))))
                    return;

            var user = await GetUserByIdAsync(userId);
            if (caseInsensitive)
                user.Roles.RemoveAll(r => r.ToLower() == role.ToLower());
            else
                user.Roles.RemoveAll(r => r == role);

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

            if (!(users.Where(u => u._Id == userId).Any(u => u.Roles.Any(r => r.ToLower() == role.ToLower()))))
                return;

            if (!caseInsensitive)
                if (!(users.Where(u => u._Id == userId).Any(u => u.Roles.Any(r => r == role))))
                    return;

            var user = GetUserById(userId);
            if (caseInsensitive)
                user.Roles.RemoveAll(r => r.ToLower() == role.ToLower());
            else
                user.Roles.RemoveAll(r => r == role);

            crudService.Replace(user);
        }
        public void RemoveRole(string userId, string[] roles, bool caseInsensitive = false)
        {
            foreach (var role in roles)
            {
                RemoveRole(userId, role, caseInsensitive);
            }
        }
    }
}
