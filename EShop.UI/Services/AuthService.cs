using EShop.UI.Models;
using EShop.UI.Services.IService;
using EShop.UI.Utility;

namespace EShop.UI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = registrationRequestDto,
                Url = StaticDetails.AuthAPIBase + "/auth/AssignRole"
            });
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = loginRequestDto,
                Url = StaticDetails.AuthAPIBase + "/auth/login"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = registrationRequestDto,
                Url = StaticDetails.AuthAPIBase + "/auth/register"

            }, withBearer: false);
        }
    }
}
