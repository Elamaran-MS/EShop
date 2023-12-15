using EShop.Services.AuthAPI.Models;

namespace EShop.Services.AuthAPI.Services.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
