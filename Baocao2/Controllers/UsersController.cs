using Baocao2.Extensions;
using Baocao2.Models;
using Baocao2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Baocao2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [ActionFilter(PERMISSION_FIX.User_LIST)]
        public ResultModel GetList([FromQuery] User_Search? user_Search)
        {
            return _userService.GetVwList(user_Search);
        }

        [HttpGet("{id}")]
        [ActionFilter(PERMISSION_FIX.User_VIEW)]
        public ResultModel GetById(Guid id)
        {
            return _userService.GetById(id);
        }

        [HttpPost]
        [ActionFilter(PERMISSION_FIX.User_ADD)]
        public ResultModel Insert([FromBody] User user)
        {
            user.UseridCreated = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            user.DateCreated = DateTime.Now;
            return _userService.Insert(user);
        }

        [HttpPut("{id}")]
        [ActionFilter(PERMISSION_FIX.User_EDIT)]
        public ResultModel Update(Guid id, [FromBody] User user)
        {
            user.UseridEdited = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            user.DateEdited = DateTime.Now;
            return _userService.Update(id, user);
        }

        [HttpDelete("{id}")]
        [ActionFilter(PERMISSION_FIX.User_DELETE)]
        public ResultModel Delete(Guid id)
        {
            return _userService.Delete(id);
        }
    }
}
