using AppCore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppCore.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<IList<ApplicationUser>> GetAllUsersAsync();
    Task<IList<ApplicationUser>> SearchUsersAsync(string query);
    Task<ApplicationUser> CreateSuperAdminAsync(string email, string password, string firstName, string lastName);
    Task<bool> UpdateUserAsync(ApplicationUser user);
    Task<bool> DeleteUserAsync(string id);
    Task<bool> SetUserActiveAsync(string id, bool isActive);
}

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserGroupService _groupService;

    public UserService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IUserGroupService groupService)
    {
        _context = context;
        _userManager = userManager;
        _groupService = groupService;
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _context.Users
            .Include(u => u.GroupMemberships)
                .ThenInclude(m => m.UserGroup)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IList<ApplicationUser>> GetAllUsersAsync()
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.DisplayName ?? u.UserName)
            .ToListAsync();
    }

    public async Task<IList<ApplicationUser>> SearchUsersAsync(string query)
    {
        query = query.ToLower();
        return await _context.Users
            .Where(u => !u.IsDeleted &&
                (u.Email!.ToLower().Contains(query) ||
                 u.UserName!.ToLower().Contains(query) ||
                 (u.DisplayName != null && u.DisplayName.ToLower().Contains(query)) ||
                 (u.FirstName != null && u.FirstName.ToLower().Contains(query)) ||
                 (u.LastName != null && u.LastName.ToLower().Contains(query))))
            .OrderBy(u => u.DisplayName ?? u.UserName)
            .ToListAsync();
    }

    public async Task<ApplicationUser> CreateSuperAdminAsync(string email, string password, string firstName, string lastName)
    {
        // Check if super admin already exists
        var existingSuperAdmin = await _context.Users.FirstOrDefaultAsync(u => u.IsSuperAdmin);
        if (existingSuperAdmin != null)
        {
            throw new InvalidOperationException("A super admin already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName,
            DisplayName = $"{firstName} {lastName}",
            IsSuperAdmin = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create super admin: {errors}");
        }

        // Add to Administrators group
        var adminGroup = await _groupService.GetGroupByNameAsync("Administrators");
        if (adminGroup != null)
        {
            await _groupService.AddUserToGroupAsync(user.Id, adminGroup.Id, true);
        }

        return user;
    }

    public async Task<bool> UpdateUserAsync(ApplicationUser user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        if (user.IsSuperAdmin)
        {
            throw new InvalidOperationException("Cannot delete the super admin user.");
        }

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetUserActiveAsync(string id, bool isActive)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        if (user.IsSuperAdmin && !isActive)
        {
            throw new InvalidOperationException("Cannot deactivate the super admin user.");
        }

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
