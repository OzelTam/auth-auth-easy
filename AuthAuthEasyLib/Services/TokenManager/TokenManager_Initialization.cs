using AuthAuthEasyLib.Interfaces;

namespace AuthAuthEasyLib.Services
{
    public partial class TokenManager<T> where T : class, IAuthUser
    {
        private readonly ICrudService<T> crudService;
        private readonly AuthService<T> authService;

        public TokenManager(ICrudService<T> crudService, AuthService<T> authService)
        {
            this.crudService = crudService;
            this.authService = authService;
        }
    }
}
