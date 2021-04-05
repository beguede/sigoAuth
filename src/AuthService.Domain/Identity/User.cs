using Microsoft.AspNetCore.Identity;
using System;

namespace AuthService.Domain.Identity
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
