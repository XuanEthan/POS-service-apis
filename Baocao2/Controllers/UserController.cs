using Baocao2.Extensions;
using Baocao2.Models;
using Baocao2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Baocao2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("users")]
        [ActionFilter(PERMISSION_FIX.User_LIST)]
        public ResultModel GetList()
        {
            return _userService.GetVwList();
        }

        [HttpGet("users/{id}")]
        [ActionFilter(PERMISSION_FIX.User_VIEW)]
        public ResultModel GetById(Guid id)
        {
            return _userService.GetById(id);
        }

        [HttpPost("users")]
        [ActionFilter(PERMISSION_FIX.User_ADD)]
        public ResultModel Insert([FromBody] User user)
        {
            return _userService.Insert(user);
        }

        [HttpPut("users/{id}")]
        [ActionFilter(PERMISSION_FIX.User_EDIT)]
        public ResultModel Update(Guid id, [FromBody] User user)
        {
            return _userService.Update(id, user);
        }

        [HttpDelete("users/{id}")]
        [ActionFilter(PERMISSION_FIX.User_DELETE)]
        public ResultModel Delete(Guid id)
        {
            return _userService.Delete(id);
        }
    }
}
