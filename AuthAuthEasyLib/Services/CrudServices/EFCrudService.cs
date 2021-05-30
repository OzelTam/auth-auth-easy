using AuthAuthEasyLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    internal class EFCrudService<T> : ICrudService<T> where T : class,IAuthUser
    {
        private readonly DbContext context;
        private readonly DbSet<T> usersSet;
        public EFCrudService(DbContext context, DbSet<T> usersSet)
        {
            this.context = context;
            this.usersSet = usersSet;
        }

        public void Add(T user)
        {
           
            usersSet.Add(user);
        }
        public void Add(IEnumerable<T> users)
        {
            foreach (var user in users)
            {
                Add(user);
            }
        }
        public async Task AddAsync(T user)
        {
            await Task.Run(()=>usersSet.Add(user));
            
        }
        public async Task AddAsync(IEnumerable<T> users)
        {
            foreach (var user in users)
            {
                await AddAsync(user);
            }
        }
        public long Delete(Expression<Func<T, bool>> expression)
        {
            var entity = usersSet.Where(expression);
            var i = entity.Count();
            usersSet.RemoveRange(entity);
            return Convert.ToInt64(i);
        }
        public async Task<long> DeleteAsync(Expression<Func<T, bool>> expression)
        {
            return await Task.Run(() => Delete(expression));
        }
        public T DeleteOne(Expression<Func<T, bool>> expression)
        {
            var entity = usersSet.FirstOrDefault(expression);
            usersSet.Remove(entity);
            return entity;
        }
        public async Task<T> DeleteOneAsync(Expression<Func<T, bool>> expression)
        {
            return await Task.Run(() => DeleteOne(expression));
        }
        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            context.SaveChanges();
            return usersSet.Where(expression);
        }
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            await context.SaveChangesAsync();
            return usersSet.Where(expression);
        }
        public IQueryable<T> FindQueriable(Expression<Func<T, bool>> expression)
        {
            context.SaveChanges();
            return usersSet.Where(expression);
        }
        public IQueryable<T> FindQueriable()
        {
            context.SaveChanges();
            return usersSet.AsQueryable();
        }
        public T Replace(T user)
        {
            var newUser = usersSet.SingleOrDefault(u => u._Id == user._Id);
            context.Entry(newUser).CurrentValues.SetValues(user);
            return newUser;
        }
        public async Task<T> ReplaceAsync(T user)
        {
            var newUser = usersSet.SingleOrDefault(u => u._Id == user._Id);
            context.Entry(newUser).CurrentValues.SetValues(user);
            await context.SaveChangesAsync();
            return newUser;
        }
    }
}
