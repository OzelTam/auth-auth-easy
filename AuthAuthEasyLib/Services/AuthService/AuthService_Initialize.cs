using AuthAuthEasyLib.Interfaces;

namespace AuthAuthEasyLib.Services
{
    public partial class AuthService<T> where T : IAuthUser
    {
        private readonly ICrudService<T> crudService;

        public TokenManager<T> TokenManager;
        private void InitializeSubServices()
        {
            this.TokenManager = new TokenManager<T>(crudService);
        }
        public AuthService(ICrudService<T> crudService)
        {
            this.crudService = crudService;
            InitializeSubServices();
        }
        public AuthService(MongoCrudServiceConfig serviceConfig)
        {
            this.crudService = new MongoCrudService<T>(serviceConfig);
            InitializeSubServices();
        }
    }
}
