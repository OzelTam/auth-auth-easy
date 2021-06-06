using AuthAuthEasyLib.Interfaces;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T>
    {
        public void LogOut(string key)
        {
            TokenManager.RemoveToken(key, 1);

            if(cache != null)
                cache.KeyDelete(new StackExchange.Redis.RedisKey(key));
        }
        public async Task LogOutAsync(string key)
        {
            await TokenManager.RemoveTokenAsync(key, 1);

            if (cache != null)
                await cache.KeyDeleteAsync(new StackExchange.Redis.RedisKey(key));
        }
    }
}
