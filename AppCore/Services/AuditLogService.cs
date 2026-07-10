using System.Security.Claims;
using System.Text.Json;
using AppCore.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppCore.Services;

public interface IAuditLogService
{
    Task LogAsync(
        string action,
        string? userId = null,
        string? entityType = null,
        int? entityId = null,
        object? oldValues = null,
        object? newValues = null,
        string? ipAddress = null,
        string? userAgent = null);
}

public class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuditLogService> logger)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogAsync(
        string action,
        string? userId = null,
        string? entityType = null,
        int? entityId = null,
        object? oldValues = null,
        object? newValues = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var effectiveUserId = userId
                ?? httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? httpContext?.User?.FindFirstValue(ClaimTypes.Name)
                ?? httpContext?.User?.Identity?.Name;
            var effectiveIpAddress = ipAddress ?? httpContext?.Connection.RemoteIpAddress?.ToString();
            var effectiveUserAgent = userAgent ?? httpContext?.Request.Headers["User-Agent"].ToString();

            var logEntry = new AuditLog
            {
                UserId = effectiveUserId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValues = SerializePayload(oldValues),
                NewValues = SerializePayload(newValues),
                IpAddress = effectiveIpAddress,
                UserAgent = effectiveUserAgent,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(logEntry);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to persist audit log action {Action}", action);
        }
    }

    private static string? SerializePayload(object? payload)
    {
        if (payload == null)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = false
            });
        }
        catch
        {
            return payload.ToString();
        }
    }
}
