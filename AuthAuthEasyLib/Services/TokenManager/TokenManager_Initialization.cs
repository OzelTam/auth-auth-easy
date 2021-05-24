using AuthAuthEasyLib.Interfaces;

namespace AuthAuthEasyLib.Services
{
    public partial class TokenManager<T> where T : IAuthUser
    {
        private readonly ICrudService<T> crudService;
        public TokenManager(ICrudService<T> crudService)
        {
            this.crudService = crudService;
        }
    }
}
