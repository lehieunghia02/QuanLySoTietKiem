
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Models.Admin;
using QuanLySoTietKiem.Repositories.Interfaces;

namespace QuanLySoTietKiem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InterestRateController : Controller
    {
        private readonly ILogger<InterestRateController> _logger;
        private readonly IInterestRateRepository _interestRateRepository;
        public InterestRateController(ILogger<InterestRateController> logger, IInterestRateRepository interestRateRepository)
        {
            _logger = logger;
            _interestRateRepository = interestRateRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var interestRates = await _interestRateRepository.GetAllInterestRatesAsync();
                if (interestRates == null || !interestRates.Any())
                {
                    return View("Empty");
                }
                return View(interestRates);
            }
            catch (Exception ex)
            {
                throw new Exception("Error message: ", ex);
            }
        }
        //GET: InterestRate/Create 
        [HttpGet]
        public IActionResult Create()
        {
            return View(new InterestRate());
        }

        //POST: InterestRate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InterestRate interestRate)
        {
            if (!ModelState.IsValid)
            {
                return View(interestRate);
            }
            try
            {
                var createInterestRate = await _interestRateRepository.CreateInterestRateAsync(interestRate);
                if (createInterestRate != null)
                {
                    ModelState.AddModelError("", "Failed to create interest rate.");
                    return View(interestRate);
                }
                ViewData["SuccessMessage"] = "Tạo lãi suất thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new Exception("Error message: ", ex);
            }
        }

        //GET: InterestRate/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var existInterestRate = await _interestRateRepository.IsInterestRateAsync(id); 
            if(!existInterestRate)
            {
                ViewData["NotFoundMessage"] = "InterestRate Not Found"; 
                return NotFound(); 
            }
            var interestRate = await _interestRateRepository.GetInterestRateAsync(id); 
            if(interestRate != null)
            {
                return RedirectToAction("Index");
            }
            return View(interestRate);
        }

        //Edit 
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var interestRate = await _interestRateRepository.GetInterestRateAsync(id);
                if (interestRate == null)
                {
                    return NotFound();
                }
                return View(interestRate);
            }
            catch(Exception ex)
            {
                throw new Exception($"{ex.Message}", ex);
            }   
        }

        //Edit 
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, InterestRate interestRate)
        //{
        //    if(id != interestRate.Id)
        //    {
        //        return NotFound(); 
        //    }
        //    if(!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }
        //    var entity = await _interestRateRepository.GetInterestRateAsync(id); 
        //    if(entity == null)
        //    {
        //        return NotFound();
        //    }
        //    var updateEntity = await _interestRateRepository.UpdateInterestRateAsync(interestRate); 
        //    if(updateEntity != null)
        //    {
                
        //    }

        //}

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInterestRateConfirm(int id)
        {
            var interestRate = await _interestRateRepository.GetInterestRateAsync(id); 
            if(interestRate== null)
            {
                return NotFound();
            }
            return View(); 
        }

    }
}
