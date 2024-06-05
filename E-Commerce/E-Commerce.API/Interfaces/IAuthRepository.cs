using Microsoft.AspNetCore.Identity;

namespace E_Commerce.API.Interfaces
{
    public interface IAuthRepository
    {
        Task<IdentityResult> RegisterAsync(RegisterDto model);
        Task<string> LoginAsync(LoginDto model);
    }
}
