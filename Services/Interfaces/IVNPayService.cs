using Microsoft.AspNetCore.Http;
using QuanLySoTietKiem.Models;

namespace QuanLySoTietKiem.Services.Interfaces
{
    public interface IVNPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentInformationModel PaymentExecute(IQueryCollection collections);
    }
}