using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Threading.Tasks;
using WebApplication1.Services;
namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserService _userService;
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _userService = new UserService();
    }

    public async Task<IActionResult> Index()
    {
        string ip = Request.Headers["X-Forwarded-For"];
        if (string.IsNullOrEmpty(ip))
        {
            ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        await _userService.SaveUserAsync(ip);

        return View();
    }

    public async Task<IActionResult> Privacy()
    {
        string ip = Request.Headers["X-Forwarded-For"];
        if (string.IsNullOrEmpty(ip))
        {
            ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        var user = await _userService.GetUserByIPAsync(ip);
        return View(user);
    }
    public  IActionResult Details()
    {
        return View("~/Views/Details/index.cshtml");
    }
    [HttpPost]
    public async Task<IActionResult> SaveNameCountry(string name, string country)
    {
        await _userService.SaveNameCountryAsync(name, country);
        ModelState.Clear();
        return View("~/Views/Details/index.cshtml");
    }

    public async Task<IActionResult> ViewDetails()
    {
        var names = await _userService.GetAllNameCountryAsync();
        return View("~/Views/ViewDetails/Index.cshtml", names);
    }

    public async Task<IActionResult> UserDetail(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        return View("~/Views/ViewDetails/UserDetail.cshtml", user);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
