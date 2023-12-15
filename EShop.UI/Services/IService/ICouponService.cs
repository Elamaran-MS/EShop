using EShop.UI.Models;

namespace EShop.UI.Services.IService
{
    public interface ICouponService
    {
        Task<ResponseDto?> GetAllCouponAsync();
        Task<ResponseDto?> GetCouponAsync(string couponCode);        
        Task<ResponseDto?> GetCouponByIdAsync(int id);
        Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto);
        Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto);
        Task<ResponseDto?> DeleteCouponAsync(int id);
    }
}
