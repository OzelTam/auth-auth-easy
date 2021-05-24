using AuthAuthEasyLib.Interfaces;
using System.Threading.Tasks;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T> where T : IAuthUser
    {
        public void LogOut(string key)
        {
            TokenManager.RemoveToken(key, 1);
        }
        public async Task LogOutAsync(string key)
        {
            await TokenManager.RemoveTokenAsync(key, 1);
        }
    }
}
