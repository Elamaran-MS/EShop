using AutoMapper;
using EShop.Services.ShoppingCartAPI.Data;
using EShop.Services.ShoppingCartAPI.Models;
using EShop.Services.ShoppingCartAPI.Models.Dto;
using EShop.Services.ShoppingCartAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace EShop.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _responseDto;
        private IMapper _mapper;
        private IProductService _productService;
        private ICouponService _couponService;

        public CartAPIController(AppDbContext db, IMapper mapper, 
            IProductService productService, ICouponService couponService)
        {
            _db = db;
            _mapper = mapper;
            _productService = productService;
            _responseDto = new ResponseDto();
            _couponService = couponService;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u=>u.UserId==userId))
                };

                cartDto.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails
                    .Where(u=>u.CartHeaderId==cartDto.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

                foreach(var item in cartDto.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u=>u.ProductId==item.ProductId);
                    cartDto.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                //apply coupon if any
                if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    CouponDto couponDto = await _couponService.GetCoupon(cartDto.CartHeader.CouponCode);
                    if(couponDto != null && cartDto.CartHeader.CartTotal > couponDto.MinAmount)
                    {
                        cartDto.CartHeader.CartTotal -= couponDto.DiscountAmount;
                        cartDto.CartHeader.Discount = couponDto.DiscountAmount;
                    }
                }
                _responseDto.Result = cartDto;
            }
            catch (Exception ex)
            {
                _responseDto.isSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(u=>u.UserId==cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.isSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = "";
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.isSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //Create Header and Details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //if header is not null check if details has same product
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u=>u.ProductId==cartDto.CartDetails.First().ProductId &&
                        u.CartHeaderId==cartHeaderFromDb.CartHeaderId);
                    if(cartDetailsFromDb == null)
                    {
                        //Create CartDetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }
                _responseDto.Result = cartDto;
            }
            catch (Exception ex)
            {
                _responseDto.isSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody]int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails.First(u=>u.CartDetailsId==cartDetailsId);
                int totalCountofCartItem = _db.CartDetails.Where(u=>u.CartHeaderId==cartDetails.CartHeaderId).Count();
                
                _db.CartDetails.Remove(cartDetails);

                if (totalCountofCartItem == 1)
                {
                    var cartHeadertoRemove = await _db.CartHeaders.FirstOrDefaultAsync(
                        u=>u.CartHeaderId==cartDetails.CartHeaderId);
                    _db.CartHeaders.Remove(cartHeadertoRemove);
                }

                await _db.SaveChangesAsync();
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.isSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }
    }
}
