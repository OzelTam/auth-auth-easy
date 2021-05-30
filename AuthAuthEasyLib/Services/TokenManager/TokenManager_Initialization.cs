using AuthAuthEasyLib.Interfaces;

namespace AuthAuthEasyLib.Services
{
    public partial class TokenManager<T> where T : class, IAuthUser
    {
        private readonly ICrudService<T> crudService;
        public TokenManager(ICrudService<T> crudService)
        {
            this.crudService = crudService;
        }
    }
}
