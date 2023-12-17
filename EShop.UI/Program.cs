using EShop.UI.Services;
using EShop.UI.Services.IService;
using EShop.UI.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EShop.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient<ICouponService, CouponService>();
            builder.Services.AddHttpClient<ICartService, CartService>();
            builder.Services.AddHttpClient<IProductService, ProductService>();
            builder.Services.AddHttpClient<IAuthService, AuthService>();

            StaticDetails.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];
            StaticDetails.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
            StaticDetails.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];
            StaticDetails.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"];

            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<IBaseService, BaseService>();
            builder.Services.AddScoped<ICouponService, CouponService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromHours(10);
                    options.LoginPath = "/Auth/Login";
                    options.AccessDeniedPath = "/Auth/AccessDenied";
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
