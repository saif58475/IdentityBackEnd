using TestApi.Models;

namespace TestApi.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> Login(SignInModel model);
    }
}
