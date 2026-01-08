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
    public class RolesController : ControllerBase
    {
        private readonly RoleService _roleService;
        private readonly RolePermissionService _rolePermissionService;
        private readonly UserService _userService;

        public RolesController(RoleService roleService , RolePermissionService rolePermissionService , UserService userService)
        {
            _roleService = roleService;
            _rolePermissionService = rolePermissionService;
            _userService = userService;
        }

        [HttpGet]
        [ActionFilter(PERMISSION_FIX.Role_LIST , PERMISSION_FIX.User_ADD , PERMISSION_FIX.User_EDIT)]
        public ResultModel GetList()
        {
            return _roleService.GetList();
        }

        [HttpGet("{id}")]
        [ActionFilter(PERMISSION_FIX.Role_VIEW)]
        public ResultModel GetById(Guid id)
        {
            return _roleService.GetById(id);
        }

        [HttpPost]
        [ActionFilter(PERMISSION_FIX.Role_ADD)]
        public ResultModel Insert([FromBody] Role role)
        {
            return _roleService.Insert(role);
        }

        [HttpPut("{id}")]
        [ActionFilter(PERMISSION_FIX.Role_EDIT)]
        public ResultModel Update(Guid id, [FromBody] Role role)
        {
            return _roleService.Update(id, role);
        }

        [HttpDelete("{id}")]
        [ActionFilter(PERMISSION_FIX.Role_DELETE)]
        public ResultModel Delete(Guid id) // roleId
        {
            if(_userService.isRoleUsed(id))
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.NotOK, Message = "Vai trò đang được sử dụng, không thể xóa", Id = null, Object = null };
            }
            var xoaRolePermissionsRes = _rolePermissionService.Delete(id);
            if (!xoaRolePermissionsRes.IsSuccess)
            {
                return xoaRolePermissionsRes;
            }
            return _roleService.Delete(id);
        }
    }
}
