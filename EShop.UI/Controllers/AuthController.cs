using EShop.UI.Models;
using EShop.UI.Services.IService;
using EShop.UI.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace EShop.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            ResponseDto responseDto = await _authService.LoginAsync(loginRequestDto);

            if (responseDto != null && responseDto.isSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                await SignInUser(loginResponseDto);
                _tokenProvider.SetToken(loginResponseDto.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //ModelState.AddModelError("CustomError", responseDto.Message);
                TempData["error"] = responseDto.Message;
                return View(loginRequestDto);
            }            
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=StaticDetails.RoleAdmin, Value=StaticDetails.RoleAdmin },
                new SelectListItem{Text=StaticDetails.RoleCustomer, Value=StaticDetails.RoleCustomer }
            };
            ViewBag.RoleList = roleList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registerationRequestDto)
        {
            ResponseDto result = await _authService.RegisterAsync(registerationRequestDto);
            ResponseDto assignRole;
            if (result != null && result.isSuccess)
            {
                if (string.IsNullOrEmpty(registerationRequestDto.Role))
                {
                    registerationRequestDto.Role = StaticDetails.RoleCustomer;
                }
                assignRole = await _authService.AssignRoleAsync(registerationRequestDto);
                if (assignRole != null && assignRole.isSuccess)
                {
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = result.Message;
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=StaticDetails.RoleAdmin, Value=StaticDetails.RoleAdmin },
                new SelectListItem{Text=StaticDetails.RoleCustomer, Value=StaticDetails.RoleCustomer }
            };
            ViewBag.RoleList = roleList;
            return View(registerationRequestDto);
        }        

        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u=>u.Type== JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        }
    }
}
