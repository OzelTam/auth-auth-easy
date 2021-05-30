using AuthAuthEasyLib.Bases;
using AuthAuthEasyLib.Interfaces;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class TokenManager<T> where T : class, IAuthUser
    {
        public async Task AddTokenAsync(T user, Token token)
        {
            user.Tokens.Add(token);
            await crudService.ReplaceAsync(user);
        }
        public void AddToken(T user, Token token)
        {
            user.Tokens.Add(token);
            crudService.Replace(user);
        }
    }
}
