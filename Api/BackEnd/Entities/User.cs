using Microsoft.AspNetCore.Identity;

namespace BackEnd.Entities
{
    public class User : IdentityUser<int>
    {
        public UserAddress Address { get; set; }
    }
}
