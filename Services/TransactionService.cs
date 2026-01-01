using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models.GiaoDichModels;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TransactionService> _logger; 
    public TransactionService(ApplicationDbContext context, ILogger<TransactionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GiaoDich> CreateTransaction(TransactionModel model)
    {
        var transaction = new GiaoDich
        {
            MaSoTietKiem = model.MaSoTietKiem,
            MaLoaiGiaoDich = model.MaLoaiGiaoDich,
            NgayGiaoDich = model.NgayGiaoDich,
            SoTien = model.SoTien,
        };
        _context.GiaoDichs.Add(transaction);
        await _context.SaveChangesAsync();

        model.MaGiaoDich = transaction.MaGiaoDich;
        return transaction;
    }

    public async Task<List<GiaoDich>> GetAllTransactionAsync()
    {
        return await _context.GiaoDichs.Include(g => g.MaSoTietKiem).ToListAsync();
    }

    public async Task<IEnumerable<GiaoDich>> GetAllTransactionByUserIdAsync(string userId)
    {
        throw new Exception("Not implemented yet");
    }
}