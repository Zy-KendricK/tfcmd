using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly ICachedHomeContentService _homeContent;

	public HomeController(ILogger<HomeController> logger, ICachedHomeContentService homeContent)
	{
		_logger = logger;
		_homeContent = homeContent;
	}

	public async Task<IActionResult> Index()
	{
		var content = await _homeContent.GetHomePageContentAsync();
		return View(content);
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
