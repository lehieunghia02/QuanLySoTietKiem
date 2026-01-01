using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLySoTietKiem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Repositories.Implementations;
using QuanLySoTietKiem.Repositories.Interfaces;

namespace QuanLySoTietKiem.Controllers;

[Authorize(Policy = PolicyConstants.RequireUser)]
public class TransactionController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TransactionController> _logger;
    private readonly ITransactionRepository _transactionRepository; 

    [ActivatorUtilitiesConstructor]
    public TransactionController(
        UserManager<ApplicationUser> userManager,
        ITransactionRepository transactionRepository, 
        ApplicationDbContext context,
        
        ILogger<TransactionController> logger)
    {
        _userManager = userManager;
        _transactionRepository = transactionRepository;
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var savingsAccounts = await _context.SoTietKiems
        .Where(s => s.UserId == currentUser.Id)
        .Select(s => new SelectListItem
        {
            Value = s.MaSoTietKiem.ToString(),
            Text = s.Code
        })
        .ToListAsync();

        ViewBag.SavingsAccounts = new SelectList(savingsAccounts, "Value", "Text");
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> ListTransactions(int selectedAccountId)
    {
        var transactions = await _transactionRepository.GetAllTransactionsBySavingsAccountId(selectedAccountId) ?? new List<GiaoDich>();
        if(transactions == null || transactions.Count ==0)
        {
            ViewBag.Message = "Không có giao dịch nào cho sổ tiết kiệm đã chọn.";
        }else
        {
            ViewBag.Transactions = transactions;
        }
        return View(transactions);   
    }
}