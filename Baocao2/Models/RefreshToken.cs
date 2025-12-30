namespace Baocao2.Models
{
    public class RefreshToken
    {
        public string token { get; set; }
        public string jwtId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Used { get; set; } = false;
    }
    public class RefreshToken_Request
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }

    public static class RefreshTokens
    {
        public static List<RefreshToken> refreshTokens = new List<RefreshToken>
        {
        };
    }
}
