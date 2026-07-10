using AppCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppCore.Services;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string userId, string permissionCode);
    Task<bool> HasAnyPermissionAsync(string userId, params string[] permissionCodes);
    Task<bool> HasAllPermissionsAsync(string userId, params string[] permissionCodes);
    Task<IList<string>> GetUserPermissionsAsync(string userId);
    Task<IList<Permission>> GetAllPermissionsAsync();
    Task<IList<Permission>> GetPermissionsByCategoryAsync(string category);
    Task<bool> IsSuperAdminAsync(string userId);
}

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _context;

    public PermissionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasPermissionAsync(string userId, string permissionCode)
    {
        // Super admin has all permissions
        if (await IsSuperAdminAsync(userId))
            return true;

        return await _context.UserGroupMemberships
            .Where(m => m.UserId == userId && m.IsActive && !m.IsDeleted)
            .Join(_context.GroupPermissions.Where(gp => gp.IsGranted && gp.IsActive && !gp.IsDeleted),
                m => m.UserGroupId,
                gp => gp.UserGroupId,
                (m, gp) => gp.PermissionId)
            .Join(_context.Permissions.Where(p => p.Code == permissionCode && p.IsActive && !p.IsDeleted),
                permissionId => permissionId,
                p => p.Id,
                (permissionId, p) => p)
            .AnyAsync();
    }

    public async Task<bool> HasAnyPermissionAsync(string userId, params string[] permissionCodes)
    {
        if (await IsSuperAdminAsync(userId))
            return true;

        if (permissionCodes == null || permissionCodes.Length == 0)
            return false;

        return await _context.UserGroupMemberships
            .Where(m => m.UserId == userId && m.IsActive && !m.IsDeleted)
            .Join(_context.GroupPermissions.Where(gp => gp.IsGranted && gp.IsActive && !gp.IsDeleted),
                m => m.UserGroupId,
                gp => gp.UserGroupId,
                (m, gp) => gp.PermissionId)
            .Join(_context.Permissions.Where(p => permissionCodes.Contains(p.Code) && p.IsActive && !p.IsDeleted),
                permissionId => permissionId,
                p => p.Id,
                (permissionId, p) => p)
            .AnyAsync();
    }

    public async Task<bool> HasAllPermissionsAsync(string userId, params string[] permissionCodes)
    {
        if (await IsSuperAdminAsync(userId))
            return true;

        var userPermissions = await GetUserPermissionsAsync(userId);
        return permissionCodes.All(code => userPermissions.Contains(code));
    }

    public async Task<IList<string>> GetUserPermissionsAsync(string userId)
    {
        if (await IsSuperAdminAsync(userId))
        {
            return await _context.Permissions
                .Where(p => p.IsActive && !p.IsDeleted)
                .Select(p => p.Code)
                .ToListAsync();
        }

        return await _context.UserGroupMemberships
            .Where(m => m.UserId == userId && m.IsActive && !m.IsDeleted)
            .Join(_context.GroupPermissions.Where(gp => gp.IsGranted && gp.IsActive && !gp.IsDeleted),
                m => m.UserGroupId,
                gp => gp.UserGroupId,
                (m, gp) => gp.PermissionId)
            .Join(_context.Permissions.Where(p => p.IsActive && !p.IsDeleted),
                permissionId => permissionId,
                p => p.Id,
                (permissionId, p) => p.Code)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IList<Permission>> GetAllPermissionsAsync()
    {
        return await _context.Permissions
            .Where(p => p.IsActive && !p.IsDeleted)
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Module)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IList<Permission>> GetPermissionsByCategoryAsync(string category)
    {
        return await _context.Permissions
            .Where(p => p.Category == category && p.IsActive && !p.IsDeleted)
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<bool> IsSuperAdminAsync(string userId)
    {
        return await _context.Users
            .AnyAsync(u => u.Id == userId && u.IsSuperAdmin);
    }
}
