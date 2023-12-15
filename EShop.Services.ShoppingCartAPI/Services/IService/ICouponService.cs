using EShop.Services.ShoppingCartAPI.Models.Dto;

namespace EShop.Services.ShoppingCartAPI.Services.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCoupon(string couponCode);
    }
}
