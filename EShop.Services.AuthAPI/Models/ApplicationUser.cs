using Microsoft.AspNetCore.Identity;

namespace EShop.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
