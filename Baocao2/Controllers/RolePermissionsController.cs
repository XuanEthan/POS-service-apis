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
    public class RolePermissionsController : ControllerBase
    {
        private readonly RolePermissionService _rolePermissionService;

        public RolePermissionsController(RolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        [HttpGet]
        [ActionFilter(PERMISSION_FIX.Role_Permisstion_LIST)]
        public ResultModel GetList([FromQuery] RolePermission_Search rolePermission_Search)
        {
            return _rolePermissionService.GetVList(rolePermission_Search);
        }

        [HttpGet("{id}")]
        [ActionFilter(PERMISSION_FIX.Role_Permisstion_VIEW)]
        public ResultModel GetById(Guid id) // rpId
        {
            return _rolePermissionService.GetById(id);
        }

        [HttpPost]
        [ActionFilter(PERMISSION_FIX.Role_Permisstion_ADD)]
        public ResultModel Insert([FromBody] RolePermission_Save rolePermission)
        {
            return _rolePermissionService.PhanQuyen(rolePermission);
        }

        [HttpDelete("{id}")]
        [ActionFilter(PERMISSION_FIX.Role_Permisstion_DELETE)]
        public ResultModel Delete(Guid id)
        {
            return _rolePermissionService.Delete(id);
        }
    }
}
