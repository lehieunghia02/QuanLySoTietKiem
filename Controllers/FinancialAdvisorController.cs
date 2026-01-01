using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[Authorize]
public class FinancialAdvisorController : Controller
{
  private readonly IFinancialAdvisorService _advisorService;
  private readonly ILogger<FinancialAdvisorController> _logger;

  public FinancialAdvisorController(
    IFinancialAdvisorService advisorService, 
    ILogger<FinancialAdvisorController> logger)
  {
    _advisorService = advisorService;
    _logger = logger;
  }

  public IActionResult Index()
  {
    return View();
  }

  [HttpPost]
  public async Task<IActionResult> GetAdvice([FromBody] string question)
  {
    try
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(userId))
      {
        return Json(new { success = false, message = "Vui lòng đăng nhập để sử dụng tính năng này" });
      }
      var response = await _advisorService.GetFinancialAdvice(question, userId);
      return Json(new { success = true, message = response });
    }
    catch (Exception ex)
    {
      return Json(new { success = false, message = ex.Message });
    }
  }
}