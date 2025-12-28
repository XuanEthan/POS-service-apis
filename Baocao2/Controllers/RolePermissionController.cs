using Baocao2.Models;
using Baocao2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Baocao2.Controllers
{
    public class RolePermissionController : Controller
    {
        private readonly RolePermissionService _rolePermissionService;
        public RolePermissionController(RolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        [HttpGet("/role_permissions")]
        public ResultModel GetList()
        {
            var result = _rolePermissionService.GetList(null);
            return result;
        }
    }
}
