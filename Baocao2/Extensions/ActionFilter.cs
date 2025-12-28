using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Baocao2.Extensions
{
    public class ActionFilter : ActionFilterAttribute
    {
        public string _permission { get; set; }
        public ActionFilter(string permission)
        {
            _permission = permission;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(_permission)) return;
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var has = user.Claims
            .Any(c => c.Type == "permission" && c.Value == _permission);

            if (!has)
            {
                context.Result = new ForbidResult();
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
