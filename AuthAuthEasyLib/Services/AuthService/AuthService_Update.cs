using AuthAuthEasyLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T> where T:class,IAuthUser
    {
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
    }
}
