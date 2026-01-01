using Microsoft.AspNetCore.Mvc;

namespace QuanLySoTietKiem.Controllers
{
  public class ErrorController : Controller
  {
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
      switch (statusCode)
      {
        case 404:
          return View("Error404");
        default:
          return View("Error");
      }
    }
  }
}