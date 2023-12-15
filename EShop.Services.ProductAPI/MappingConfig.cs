using AutoMapper;
using EShop.Services.ProductAPI.Models;
using EShop.Services.ProductAPI.Models.Dto;

namespace EShop.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
