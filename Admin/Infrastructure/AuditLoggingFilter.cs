using System.Security.Claims;
using System.Text.Json;
using AppCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Admin.Infrastructure;

/// <summary>
/// Opts an action (or controller) out of audit logging — used by high-frequency
/// polling endpoints (e.g. messenger updates) that would otherwise flood the log.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SkipAuditLogAttribute : Attribute
{
}

public class AuditLoggingFilter : IAsyncActionFilter, IAsyncExceptionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<SkipAuditLogAttribute>().Any())
        {
            await next();
            return;
        }

        var auditLogService = context.HttpContext.RequestServices.GetRequiredService<IAuditLogService>();

        try
        {
            var executedContext = await next();
            var actionName = GetActionName(context);
            var metadata = BuildMetadata(context, executedContext.Result);

            await auditLogService.LogAsync(
                action: $"action:{actionName}",
                entityType: context.Controller?.GetType().Name,
                oldValues: null,
                newValues: metadata,
                userId: context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                ipAddress: context.HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent: context.HttpContext.Request.Headers["User-Agent"].ToString());
        }
        catch (Exception ex)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuditLoggingFilter>>();
            logger.LogError(ex, "Unhandled exception while executing action {ActionName}", GetActionName(context));
            throw;
        }
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        var auditLogService = context.HttpContext.RequestServices.GetRequiredService<IAuditLogService>();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuditLoggingFilter>>();
        var actionName = GetActionName(context);

        logger.LogError(context.Exception, "Exception raised while executing action {ActionName}", actionName);
        await auditLogService.LogAsync(
            action: $"exception:{actionName}",
            entityType: context.ActionDescriptor?.DisplayName,
            oldValues: null,
            newValues: new { error = context.Exception.Message, stackTrace = context.Exception.StackTrace },
            userId: context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
            ipAddress: context.HttpContext.Connection.RemoteIpAddress?.ToString(),
            userAgent: context.HttpContext.Request.Headers["User-Agent"].ToString());

        context.ExceptionHandled = true;
        context.Result = new RedirectToActionResult("Error", "Home", new { area = "" });
        await Task.CompletedTask;
    }

    private static object BuildMetadata(ActionExecutingContext context, object? result)
    {
        var arguments = context.ActionArguments
            .Where(kvp => !IsSensitive(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return new
        {
            controller = context.Controller?.GetType().Name,
            action = context.ActionDescriptor.RouteValues.TryGetValue("action", out var actionValue) ? actionValue?.ToString() : context.ActionDescriptor.DisplayName,
            routeValues = context.RouteData.Values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString()),
            arguments,
            resultType = result?.GetType().Name,
            statusCode = context.HttpContext.Response.StatusCode
        };
    }

    private static string GetActionName(ActionContext context)
    {
        var controllerName = context.ActionDescriptor.RouteValues.TryGetValue("controller", out var controllerValue)
            ? controllerValue?.ToString()
            : context.ActionDescriptor.DisplayName;
        var actionName = context.ActionDescriptor.RouteValues.TryGetValue("action", out var actionValue)
            ? actionValue?.ToString()
            : context.ActionDescriptor.DisplayName;

        return $"{controllerName}/{actionName}";
    }

    private static bool IsSensitive(string key)
    {
        var value = key.ToLowerInvariant();
        return value.Contains("password", StringComparison.Ordinal) || value.Contains("token", StringComparison.Ordinal) || value.Contains("secret", StringComparison.Ordinal);
    }
}
