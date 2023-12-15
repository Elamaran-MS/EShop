using AutoMapper;
using EShop.Services.ShoppingCartAPI.Models;
using EShop.Services.ShoppingCartAPI.Models.Dto;

namespace EShop.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
                config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
