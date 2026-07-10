using AppCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Admin.Controllers;

/// <summary>
/// Base controller for admin panel with permission checking
/// </summary>
public abstract class BaseAdminController : Controller
{
    protected readonly IPermissionService _permissionService;

    protected BaseAdminController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    protected string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    protected async Task<bool> HasPermissionAsync(string permissionCode)
    {
        if (string.IsNullOrEmpty(CurrentUserId))
            return false;

        return await _permissionService.HasPermissionAsync(CurrentUserId, permissionCode);
    }

    protected async Task<bool> IsSuperAdminAsync()
    {
        if (string.IsNullOrEmpty(CurrentUserId))
            return false;

        return await _permissionService.IsSuperAdminAsync(CurrentUserId);
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Check if user is authenticated
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = RedirectToAction("Login", "Account");
            return;
        }

        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();
        var requiresPasswordChange = User.HasClaim("RequirePasswordChange", "true");
        var isPasswordChangeAllowed = (controllerName == "Dashboard" && actionName == "Settings") ||
                                      (controllerName == "Account" && (actionName == "Logout" || actionName == "Login"));

        if (requiresPasswordChange && !isPasswordChangeAllowed)
        {
            context.Result = new RedirectToActionResult("Settings", "Dashboard", new { forceChange = true });
            return;
        }

        // Pass user permissions to ViewBag
        if (!string.IsNullOrEmpty(CurrentUserId))
        {
            ViewBag.UserPermissions = await _permissionService.GetUserPermissionsAsync(CurrentUserId);
            ViewBag.IsSuperAdmin = await _permissionService.IsSuperAdminAsync(CurrentUserId);
        }

        await base.OnActionExecutionAsync(context, next);
    }
}

/// <summary>
/// Attribute for requiring specific permission
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAsyncActionFilter
{
    private readonly string[] _permissionCodes;

    public RequirePermissionAttribute(params string[] permissionCodes)
    {
        _permissionCodes = permissionCodes ?? Array.Empty<string>();
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var permissionService = context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();
        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        // Super admin bypasses all permission checks
        if (await permissionService.IsSuperAdminAsync(userId))
        {
            await next();
            return;
        }

        if (_permissionCodes.Length == 0 || !await permissionService.HasAnyPermissionAsync(userId, _permissionCodes))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            return;
        }

        await next();
    }
}
