namespace EShop.Services.CouponAPI.Models.Dto
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string? CouponCode { get; set; }
        public Double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}
