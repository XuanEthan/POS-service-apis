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
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;
        
        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("roles")]
        [ActionFilter(PERMISSION_FIX.Role_LIST)]
        public ResultModel GetList()
        {
            return _roleService.GetList();
        }

        [HttpGet("roles/{id}")]
        [ActionFilter(PERMISSION_FIX.Role_VIEW)]
        public ResultModel GetById(Guid id)
        {
            return _roleService.GetById(id);
        }

        [HttpPost("roles")]
        [ActionFilter(PERMISSION_FIX.Role_ADD)]
        public ResultModel Insert([FromBody] Role role)
        {
            return _roleService.Insert(role);
        }

        [HttpPut("roles/{id}")]
        [ActionFilter(PERMISSION_FIX.Role_EDIT)]
        public ResultModel Update(Guid id, [FromBody] Role role)
        {
            return _roleService.Update(id, role);
        }

        [HttpDelete("roles/{id}")]
        [ActionFilter(PERMISSION_FIX.Role_DELETE)]
        public ResultModel Delete(Guid id)
        {
            return _roleService.Delete(id);
        }
    }
}
