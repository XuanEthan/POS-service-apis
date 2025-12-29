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
    public class RolePermissionController : ControllerBase
    {
        private readonly RolePermissionService _rolePermissionService;

        public RolePermissionController(RolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        [HttpGet("rolepermissions")]
        [ActionFilter(PERMISSION_FIX.Role_Permisstion_LIST)]
        public ResultModel GetList([FromQuery] string? roleId)
        {
            return _rolePermissionService.GetList(roleId);
        }

        [HttpGet("rolepermissions/{id}")]
        [ActionFilter(PERMISSION_FIX.Role_Permisstion_VIEW)]
        public ResultModel GetById(Guid id)
        {
            return _rolePermissionService.GetById(id);
        }

        [HttpPost("rolepermissions")]
        [ActionFilter(PERMISSION_FIX.Role_Permisstion_ADD)]
        public ResultModel Insert([FromBody] RolePermission rolePermission)
        {
            return _rolePermissionService.Insert(rolePermission);
        }

        [HttpPut("rolepermissions/{id}")]
        [ActionFilter(PERMISSION_FIX.Role_Permisstion_EDIT)]
        public ResultModel Update(Guid id, [FromBody] RolePermission rolePermission)
        {
            return _rolePermissionService.Update(id, rolePermission);
        }

        [HttpDelete("rolepermissions/{id}")]
        [ActionFilter(PERMISSION_FIX.Role_Permisstion_DELETE)]
        public ResultModel Delete(Guid id)
        {
            return _rolePermissionService.Delete(id);
        }
    }
}
