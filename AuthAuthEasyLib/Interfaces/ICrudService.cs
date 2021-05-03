using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Interfaces
{
    public interface ICrudService<T>
    {
        void Add(T user);
        void Add(IEnumerable<T> users);
        Task AddAsync(T user);
        Task AddAsync(IEnumerable<T> users);
        long Delete(Expression<Func<T, bool>> expression);
        Task<long> DeleteAsync(Expression<Func<T, bool>> expression);
        T DeleteOne(Expression<Func<T, bool>> expression);
        Task<T> DeleteOneAsync(Expression<Func<T, bool>> expression);
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
        T Replace(T user);
        Task<T> ReplaceAsync(T user);
    }
}
