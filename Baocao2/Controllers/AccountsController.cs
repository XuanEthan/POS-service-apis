using Baocao2.Models;
using Baocao2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Baocao2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _accountService;
        public AccountsController(AccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost("login")]
        public LoginModel login([FromBody] User_Login user_Login)
        {
            return _accountService.Login(user_Login.userName , user_Login.pwd , user_Login.rememberMe);
        }

        [HttpPost("refreshtoken")]
        public LoginModel RefreshToken([FromBody] RefreshToken_Request req)
        {
            return _accountService.RefreshToken(req.accessToken , req.refreshToken);
        }
    }
}
