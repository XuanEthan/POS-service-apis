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
    public class PermissionsController : ControllerBase
    {
        private readonly PermissionService _permissionService;

        public PermissionsController(PermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        [ActionFilter(PERMISSION_FIX.Permission_LIST)]
        public ResultModel GetList()
        {
            return _permissionService.GetList();
        }
    }
}
