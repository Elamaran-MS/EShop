using EShop.Services.ShoppingCartAPI.Models.Dto;

namespace EShop.Services.ShoppingCartAPI.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
