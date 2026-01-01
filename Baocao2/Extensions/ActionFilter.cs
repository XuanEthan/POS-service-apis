using Baocao2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Baocao2.Extensions
{
    public class ActionFilter : ActionFilterAttribute
    {
        private readonly string[] _permissions;
        
        public ActionFilter(params string[] permissions)
        {
            _permissions = permissions ?? Array.Empty<string>();
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_permissions == null || _permissions.Length == 0) 
                return;
            
            var user = context.HttpContext.User;

            // ✅ SỬA 1: Kiểm tra xác thực
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

                // ✅ SỬA 2: Kiểm tra admin đúng cách
            // Admin có role code = "QUANTRI" hoặc "ADMIN"
            var isAdmin = user.Claims.Any(c => 
                c.Type == "roleId" && c.Value == Role_Fix.ADMIN);
            
            if (isAdmin)
            {
                return; // Admin bypass tất cả permission checks
            }

            // ✅ SỬA 3: Kiểm tra user có ít nhất 1 permission
            var hasAnyPermission = false;
            
            foreach (var permission in _permissions)
            {
                if (string.IsNullOrEmpty(permission))
                    continue;

                var hasPerm = user.Claims.Any(c => 
                    c.Type == "permission" && c.Value == permission);
                
                if (hasPerm)
                {
                    hasAnyPermission = true;
                    break;
                }
            }

            // ✅ SỬA 4: Return Forbid nếu không có quyền
            if (!hasAnyPermission)
            {
                context.Result = new ForbidResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
