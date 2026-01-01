using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Repositories.Interfaces;

namespace QuanLySoTietKiem.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IInterestRateRepository _interestRateRepository;

        public HomeController(ILogger<HomeController> logger, 
            UserManager<ApplicationUser> userManager, 
            IInterestRateRepository interestRateRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _interestRateRepository  = interestRateRepository;  
        }
        private string GetUserNameByCurrentUser(ApplicationUser user)
        {
            return user.UserName ?? "Khách";
        }
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Trang chủ");
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                ViewBag.ThongBao = "Bạn chưa đăng nhập, vui lòng đăng nhập !!!";
                return View();
            }
            string userName = GetUserNameByCurrentUser(currentUser);            
            ViewBag.UserName = userName;

            var checkRole =
                await _userManager.IsInRoleAsync(currentUser ?? throw new Exception("User not found"), "User");
            var checkRoleAdmin =
                await _userManager.IsInRoleAsync(currentUser ?? throw new Exception("User not found"), "Admin");
            if (checkRole)
            {
                return RedirectToAction("Index", "User");
            }
            else if (checkRoleAdmin)
            {
                return RedirectToAction("Index", "Admin");
            }

            return View();
        }

        [HttpGet]
        public ActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public ActionResult TestEmail()
        {
            return View();
        }


        [HttpGet]
        public ActionResult GioiThieu()
        {
            ViewBag.InterestRates = _interestRateRepository.GetAllInterestRatesAsync();
            return View();
        }
    }
}