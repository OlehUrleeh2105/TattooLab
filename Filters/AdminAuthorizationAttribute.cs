using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TatooLab.Filters;

public class AdminAuthorizationAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.User.IsInRole("Admin"))
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);

        base.OnActionExecuting(context);
    }
}