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
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;
        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost("/login")]
        public LoginModel login(string username, string password, string? rememberMe)
        {
            return _accountService.Login(username, password, rememberMe);
        }

        [HttpPost("/refreshtoken")]
        public LoginModel RefreshToken(string accessToken , string refreshToken)
        {
            return _accountService.RefreshToken(accessToken , refreshToken);
        }
    }
}
