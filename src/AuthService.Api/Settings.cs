namespace Kudobox.Api
{
    //public static class Settings
    //{
    //    public static string Secret = "d74d957eb5176320512993ed3909827d93ebd69f4fac714deec173af1c320fd6";
    //}

    public class JwtSettings
    {
        public string Issuer { get; set; }

        public string Secret { get; set; }

        public int ExpirationInDays { get; set; }
    }
}
