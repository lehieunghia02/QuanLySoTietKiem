using Microsoft.AspNetCore.Mvc;

namespace QuanLySoTietKiem.Controllers;

public class HuongDanController : Controller
{
    private readonly ILogger<HuongDanController> logger;
    public HuongDanController(ILogger<HuongDanController> _logger)
    {
        logger = _logger;
    }
    public IActionResult Index()
    {
        return View();
    }
}