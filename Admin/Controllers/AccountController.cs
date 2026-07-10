using System.Security.Claims;
using Admin.Models;
using AppCore;
using AppCore.Entities;
using AppCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IAuditLogService auditLogService,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var loginIdentifier = model.Email.Trim();
        var user = await _userManager.FindByEmailAsync(loginIdentifier);
        if (user == null)
        {
            user = await _userManager.FindByNameAsync(loginIdentifier);
        }

        if (user == null)
        {
            await _auditLogService.LogAsync(
                action: "login.failed.user-not-found",
                newValues: new { loginIdentifier, returnUrl },
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent: Request.Headers["User-Agent"].ToString());

            ModelState.AddModelError(string.Empty, "We couldn't find an account with that email or username. Please verify the details and try again.");
            return View(model);
        }

        if (!user.IsActive)
        {
            await _auditLogService.LogAsync(
                action: "login.failed.inactive-user",
                userId: user.Id,
                newValues: new { loginIdentifier, returnUrl },
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent: Request.Headers["User-Agent"].ToString());

            ModelState.AddModelError(string.Empty, "Your account has been deactivated. Please contact an administrator.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var requiresPasswordChange = (await _userManager.GetClaimsAsync(user)).Any(c => c.Type == "RequirePasswordChange" && c.Value == "true");

            await _auditLogService.LogAsync(
                action: requiresPasswordChange ? "login.success.password-change-required" : "login.success",
                userId: user.Id,
                newValues: new { loginIdentifier, returnUrl, requiresPasswordChange },
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent: Request.Headers["User-Agent"].ToString());

            if (requiresPasswordChange)
            {
                return RedirectToAction("Settings", "Dashboard", new { forceChange = true });
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        if (result.IsLockedOut)
        {
            await _auditLogService.LogAsync(
                action: "login.failed.locked-out",
                userId: user.Id,
                newValues: new { loginIdentifier, returnUrl },
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                userAgent: Request.Headers["User-Agent"].ToString());

            ModelState.AddModelError(string.Empty, "Account locked out. Please try again later.");
            return View(model);
        }

        await _auditLogService.LogAsync(
            action: "login.failed.credentials",
            userId: user.Id,
            newValues: new { loginIdentifier, returnUrl },
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
            userAgent: Request.Headers["User-Agent"].ToString());

        ModelState.AddModelError(string.Empty, "The email/username or password you entered is incorrect. Please try again.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _signInManager.SignOutAsync();
        await _auditLogService.LogAsync(
            action: "logout",
            userId: userId,
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
            userAgent: Request.Headers["User-Agent"].ToString());
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}