namespace QuanLySoTietKiem.Configurations
{
    public class GoogleAuthSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string AuthorizationEndpoint { get; set; } = "https://accounts.google.com/o/oauth2/v2/auth";
        public string TokenEndpoint { get; set; } = "https://oauth2.googleapis.com/token";
        public string UserInfoEndpoint { get; set; } = "https://www.googleapis.com/oauth2/v3/userinfo";

        public string[] Scopes { get; set; } = new[]
        {
            "openid",
            "profile",
            "email"
        };
    }
}