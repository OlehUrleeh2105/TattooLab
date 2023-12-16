using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TatooLab.Filters;

public class MasterAuthorizationAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.User.IsInRole("Admin") && !context.HttpContext.User.IsInRole("Master"))
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);

        base.OnActionExecuting(context);
    }
}