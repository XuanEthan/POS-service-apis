using Baocao2.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Baocao2.Services
{
    public class AccountService
    {
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private RolePermissionService _rolePermissonService;
        public AccountService(IConfiguration configuration, TokenValidationParameters tokenValidationParameters, RolePermissionService rolePermissonService)
        {
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
            _rolePermissonService = rolePermissonService;
        }

        public LoginModel Login(string username, string password, string? rememberMe)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return new LoginModel(false, ResultModel.ResultCode.Acc_Or_Pass_Does_Not_Valid, ResultModel.BuildMessage(ResultModel.ResultCode.Acc_Or_Pass_Does_Not_Valid), Guid.Empty, null, "", "");
            }
            var found = Users.UserList.FirstOrDefault(u => u.UserName == username);
            if (found == null)
            {
                return new LoginModel(false, ResultModel.ResultCode.Acc_Or_Pass_Does_Not_Correct, ResultModel.BuildMessage(ResultModel.ResultCode.Acc_Or_Pass_Does_Not_Correct), Guid.Empty, null, "", "");
            }
            if (found.Password != password)
            {
                return new LoginModel(false, ResultModel.ResultCode.Acc_Or_Pass_Does_Not_Correct, ResultModel.BuildMessage(ResultModel.ResultCode.Acc_Or_Pass_Does_Not_Correct), Guid.Empty, null, "", "");
            }
            return generateJwtTokens(found);
        }
        private LoginModel generateJwtTokens(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var perList = _rolePermissonService.GetListQuery(null).ToList();
            try
            {
                var secretKey = Encoding.UTF8.GetBytes(_configuration["JWTSettings:Secret"]);

                var claims = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier , user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role,user.RoleCode),
                    new Claim("roleId" , user.RoleId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                });

                foreach (var item in perList)
                {
                    claims.AddClaim(new Claim("permission", item.PermissionId.ToString()));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var accessToken = tokenHandler.WriteToken(token); // dừng lại ở serilize thành JWT

                var refrestToken = new RefreshToken // lưu trong ĐB
                {
                    token = Guid.NewGuid().ToString(),
                    jwtId = token.Id,
                    CreateTime = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(4)
                };

                RefreshTokens.refreshTokens.Add(refrestToken);
                return new LoginModel(true, ResultModel.ResultCode.Ok, "Đăng nhập thành công", Guid.Empty, null, accessToken, refrestToken.token);
            }
            catch (Exception ex)
            {
                return new LoginModel(true, ResultModel.ResultCode.Ok, "Đăng nhập không thành công", Guid.Empty, null, string.Empty, string.Empty);
            }
        }
        public LoginModel RefreshToken(string token, string refreshToken)
        {
            return GetRefreshToken(token, refreshToken);
        }

        private LoginModel GetRefreshToken(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
            {
                return null;
            }
            var expDateUnix = long.Parse(validatedToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expDateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(expDateUnix).UtcDateTime;
            if (expDateTimeUtc > DateTime.UtcNow)
            {
                return null;
            }
            var jti = validatedToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken = RefreshTokens.refreshTokens.FirstOrDefault(x => x.token == refreshToken); // tìm rf token trong db
            if (storedRefreshToken == null)
            {
                return null;
            }
            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate) // kt rf token hết hạn
            {
                return null;
            }
            if (storedRefreshToken.Used == true)
            {
                return null;
            }

            if (storedRefreshToken.jwtId != jti)
            {
                return null;
            }
            storedRefreshToken.Used = true;
            foreach (var rt in RefreshTokens.refreshTokens)
            {
                if (rt.token == storedRefreshToken.token)
                {
                    rt.Used = true;
                }
            }
            var userId = validatedToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var found = Users.UserList.FirstOrDefault(u => u.UserId.Equals(userId));
            return generateJwtTokens(found);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var tokenValidParameters = _tokenValidationParameters.Clone();
                tokenValidParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidParameters, out var validatedToken);
                if (!(validatedToken is JwtSecurityToken jwtSecurityToken &&
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }

    }
}
