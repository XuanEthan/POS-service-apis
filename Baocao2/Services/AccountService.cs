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
        public AccountService(IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
        {
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public LoginModel Login(string username, string password, bool? rememberMe)
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
            if(found.StatusId == User_StatusId.UnActive)
            {
                return new LoginModel(false, ResultModel.ResultCode.Acc_Is_UnActive, ResultModel.BuildMessage(ResultModel.ResultCode.Acc_Is_UnActive), Guid.Empty, null, "", "");
            }
            if(found.StatusId == User_StatusId.Block)
            {
                return new LoginModel(false, ResultModel.ResultCode.Acc_Lock_By_Admin, ResultModel.BuildMessage(ResultModel.ResultCode.Acc_Lock), Guid.Empty, null, "", "");
            }
            if (found.Password != password) // Chua hash
            {
                return new LoginModel(false, ResultModel.ResultCode.Acc_Or_Pass_Does_Not_Correct, ResultModel.BuildMessage(ResultModel.ResultCode.Acc_Or_Pass_Does_Not_Correct), Guid.Empty, null, "", "");
            }
            return generateJwtTokens(found);
        }
        private LoginModel generateJwtTokens(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //var perList = _rolePermissonService.GetListQuery(user.RoleId.ToString()).ToList();
            var perList = RolePermissions.List.Where(rp => rp.RoleId == user.RoleId).ToList();
            var role = Roles.roles.FirstOrDefault(r => r.RoleId == user.RoleId);
            try
            {
                var secretKey = Encoding.UTF8.GetBytes(_configuration["JWTSettings:Secret"]);
                var claims = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier , user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, role.Code),
                    new Claim("isAdmin" , role.Code == "QUANTRI" ? "true" : "false"),
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
                    Expires = DateTime.UtcNow.AddMinutes(2), // 1 phút
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var accessToken = tokenHandler.WriteToken(token); // dừng lại ở serilize thành JWT

                var refrestToken = new RefreshToken // lưu trong ĐB
                {
                    token = Guid.NewGuid().ToString(),
                    jwtId = token.Id,
                    CreateTime = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(3) // 4 phút
                };

                RefreshTokens.refreshTokens.Add(refrestToken);
                return new LoginModel(true, ResultModel.ResultCode.Ok, "Đăng nhập thành công", Guid.Empty, null, accessToken, refrestToken.token);
            }
            catch (Exception ex)
            {
                return new LoginModel(false, ResultModel.ResultCode.Ok, "Đăng nhập không thành công", Guid.Empty, null, string.Empty, string.Empty);
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
                return new LoginModel(false, ResultModel.ResultCode.NotOK, "Token không hợp lệ", Guid.Empty, null, null, null);
            }

            var expDateUnix = long.Parse(validatedToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expDateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(expDateUnix).UtcDateTime;
            if (expDateTimeUtc > DateTime.UtcNow)
            {
                return new LoginModel(false, ResultModel.ResultCode.NotOK, "Token chưa hết hạn", Guid.Empty, null, null, null);
            }

            var jti = validatedToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken = RefreshTokens.refreshTokens.FirstOrDefault(x => x.token == refreshToken);
            if (storedRefreshToken == null)
            {
                return new LoginModel(false, ResultModel.ResultCode.Does_Not_Exists, "Refresh token không tồn tại", Guid.Empty, null, null, null);
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new LoginModel(false, ResultModel.ResultCode.NotOK, "Refresh token đã hết hạn", Guid.Empty, null, null, null);
            }

            if (storedRefreshToken.Used == true)
            {
                //do some thing ...
                return new LoginModel(false, ResultModel.ResultCode.NotOK, "Refresh token đã được sử dụng", Guid.Empty, null, null, null);

            }

            if (storedRefreshToken.jwtId != jti)
            {
                return new LoginModel(false, ResultModel.ResultCode.NotOK, "Token không khớp với refresh token", Guid.Empty, null, null, null);
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
            var found = Users.UserList.FirstOrDefault(u => u.UserId.ToString() == userId);

            if (found == null)
            {
                return new LoginModel(false, ResultModel.ResultCode.Acc_Does_Not_Exists, "Người dùng không tồn tại", Guid.Empty, null, null, null);
            }

            return generateJwtTokens(found);
        }

        private ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var tokenValidParameters = _tokenValidationParameters.Clone();
                tokenValidParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidParameters, out var validatedToken); //validate token ...
                if (!(validatedToken is JwtSecurityToken jwtSecurityToken && // cú pháp nhanh kt tên thuật tóan
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return null;
                }
                return principal;
            } // chả về ClaimsPrincipal chứa danh tínhs (mặc định là first)
            catch
            {
                return null;
            }
        }

    }
}
