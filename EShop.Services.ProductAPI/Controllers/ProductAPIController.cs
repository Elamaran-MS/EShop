using AutoMapper;
using Azure;
using EShop.Services.ProductAPI.Data;
using EShop.Services.ProductAPI.Models;
using EShop.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _responseDto;
        private IMapper _mapper;

        public ProductAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _responseDto = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> products = _db.Products.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<ProductDto>>(products);                
            }
            catch (Exception ex)
            {
                _responseDto.isSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Product product = _db.Products.First(p=>p.ProductId==id);
                _responseDto.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _responseDto.isSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] ProductDto productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);
                _db.Products.Add(product);
                _db.SaveChanges();

                _responseDto.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _responseDto.isSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] ProductDto productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);
                _db.Products.Update(product);
                _db.SaveChanges();

                _responseDto.Result = _mapper.Map<ProductDto>(product);

            }
            catch (Exception ex)
            {
                _responseDto.isSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Product product = _db.Products.First(p=>p.ProductId==id);
                _db.Products.Remove(product);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _responseDto.isSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
    }
}
