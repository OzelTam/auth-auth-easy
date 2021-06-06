using AuthAuthEasyLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T> where T : class, IAuthUser
    {
        public async Task<T> GetUserByIdAsync(string userId)
        {
            var res = RemoveExpiredUserRoles((await crudService.FindAsync(usr => usr._Id == userId)).FirstOrDefault());
            return res ?? throw new KeyNotFoundException("User not found.");
        }
        public T GetUserById(string userId)
        {
            var res = RemoveExpiredUserRoles(crudService.Find(usr => usr._Id == userId).FirstOrDefault());
            return res ?? throw new KeyNotFoundException("User not found.");
        }
        public IEnumerable<T> FindUser(Expression<Func<T, bool>> expression)
        {
            return crudService.Find(expression);
        }
        public async Task<IEnumerable<T>> FindUserAsync(Expression<Func<T, bool>> expression)
        {
            return await crudService.FindAsync(expression);
        }
        public IQueryable<T> QueryUsers()
        {
            return crudService.FindQueriable();
        }
        public IQueryable<T> QueryUsers(Expression<Func<T, bool>> expression)
        {
            return crudService.FindQueriable(expression);
        }
        public async Task<IQueryable<T>> QueryUsersAsync()
        {
            var func = new Func<IQueryable<T>>(() => crudService.FindQueriable());
            return await Task.Run(func);
        }
        public async Task<IQueryable<T>> QueryUsersAsync(Expression<Func<T, bool>> expression)
        {
            var func = new Func<IQueryable<T>>(() => crudService.FindQueriable(expression));
            return await Task.Run(func);
        }

    }
}
