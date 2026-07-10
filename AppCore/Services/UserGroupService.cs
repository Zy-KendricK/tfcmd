using AppCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppCore.Services;

public interface IUserGroupService
{
    Task<IList<UserGroup>> GetAllGroupsAsync();
    Task<UserGroup?> GetGroupByIdAsync(int id);
    Task<UserGroup?> GetGroupByNameAsync(string name);
    Task<UserGroup> CreateGroupAsync(UserGroup group);
    Task<UserGroup> UpdateGroupAsync(UserGroup group);
    Task DeleteGroupAsync(int id);
    Task<IList<ApplicationUser>> GetGroupMembersAsync(int groupId);
    Task AddUserToGroupAsync(string userId, int groupId, bool isPrimary = false);
    Task RemoveUserFromGroupAsync(string userId, int groupId);
    Task<IList<UserGroup>> GetUserGroupsAsync(string userId);
    Task SetGroupPermissionsAsync(int groupId, IList<int> permissionIds);
    Task<IList<Permission>> GetGroupPermissionsAsync(int groupId);
}

public class UserGroupService : IUserGroupService
{
    private readonly ApplicationDbContext _context;

    public UserGroupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IList<UserGroup>> GetAllGroupsAsync()
    {
        return await _context.UserGroups
            .Where(g => g.IsActive && !g.IsDeleted)
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    public async Task<UserGroup?> GetGroupByIdAsync(int id)
    {
        return await _context.UserGroups
            .Include(g => g.Permissions)
                .ThenInclude(gp => gp.Permission)
            .Include(g => g.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(g => g.Id == id && g.IsActive && !g.IsDeleted);
    }

    public async Task<UserGroup?> GetGroupByNameAsync(string name)
    {
        return await _context.UserGroups
            .FirstOrDefaultAsync(g => g.Name == name && g.IsActive && !g.IsDeleted);
    }

    public async Task<UserGroup> CreateGroupAsync(UserGroup group)
    {
        group.CreatedAt = DateTime.UtcNow;
        _context.UserGroups.Add(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task<UserGroup> UpdateGroupAsync(UserGroup group)
    {
        group.UpdatedAt = DateTime.UtcNow;
        _context.UserGroups.Update(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task DeleteGroupAsync(int id)
    {
        var group = await _context.UserGroups.FindAsync(id);
        if (group != null)
        {
            if (group.IsSystemGroup)
            {
                throw new InvalidOperationException("Cannot delete system groups.");
            }

            group.IsDeleted = true;
            group.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IList<ApplicationUser>> GetGroupMembersAsync(int groupId)
    {
        return await _context.UserGroupMemberships
            .Where(m => m.UserGroupId == groupId && m.IsActive && !m.IsDeleted)
            .Select(m => m.User)
            .ToListAsync();
    }

    public async Task AddUserToGroupAsync(string userId, int groupId, bool isPrimary = false)
    {
        var existing = await _context.UserGroupMemberships
            .FirstOrDefaultAsync(m => m.UserId == userId && m.UserGroupId == groupId);

        if (existing != null)
        {
            if (existing.IsDeleted)
            {
                existing.IsDeleted = false;
                existing.IsActive = true;
                existing.IsPrimaryGroup = isPrimary;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            return;
        }

        var membership = new UserGroupMembership
        {
            UserId = userId,
            UserGroupId = groupId,
            IsPrimaryGroup = isPrimary,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserGroupMemberships.Add(membership);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveUserFromGroupAsync(string userId, int groupId)
    {
        var membership = await _context.UserGroupMemberships
            .FirstOrDefaultAsync(m => m.UserId == userId && m.UserGroupId == groupId);

        if (membership != null)
        {
            membership.IsDeleted = true;
            membership.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IList<UserGroup>> GetUserGroupsAsync(string userId)
    {
        return await _context.UserGroupMemberships
            .Where(m => m.UserId == userId && m.IsActive && !m.IsDeleted)
            .Select(m => m.UserGroup)
            .ToListAsync();
    }

    public async Task SetGroupPermissionsAsync(int groupId, IList<int> permissionIds)
    {
        // Remove existing permissions
        var existingPermissions = await _context.GroupPermissions
            .Where(gp => gp.UserGroupId == groupId)
            .ToListAsync();

        _context.GroupPermissions.RemoveRange(existingPermissions);

        // Add new permissions
        foreach (var permissionId in permissionIds)
        {
            var groupPermission = new GroupPermission
            {
                UserGroupId = groupId,
                PermissionId = permissionId,
                IsGranted = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.GroupPermissions.Add(groupPermission);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IList<Permission>> GetGroupPermissionsAsync(int groupId)
    {
        return await _context.GroupPermissions
            .Where(gp => gp.UserGroupId == groupId && gp.IsGranted && gp.IsActive && !gp.IsDeleted)
            .Select(gp => gp.Permission)
            .ToListAsync();
    }
}
