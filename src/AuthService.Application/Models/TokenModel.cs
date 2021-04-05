using System;

namespace AuthService.Application.Models
{
    public class TokenModel
    {
        public string Token { get; set; }
        public DateTime? Expires { get; set; }
    }
}
