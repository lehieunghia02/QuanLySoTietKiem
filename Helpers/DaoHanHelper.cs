using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;

namespace QuanLySoTietKiem.Helpers;
public static class DaoHanHelper
{
    public static async Task XuLyDaoHan(SavingAccount savingAccount, decimal interest, ApplicationDbContext context)
    {
        var user = await context.Users.FindAsync(savingAccount.UserId);
        if (user == null)
            throw new Exception("Không tìm thấy thông tin người dùng");

        switch (savingAccount.MaHinhThucDenHan)
        {
            case 1:
                await RutHet(savingAccount, interest, user, context);
                break;
            case 2:
                await QuayVongGoc(savingAccount, interest, user, context);
                break;
            case 3:
                await QuayVongGocVaLai(savingAccount, interest, context);
                break;
            default:
                throw new ArgumentException("Hình thức đáo hạn không hợp lệ");
        }
    }

    private static async Task RutHet(SavingAccount savingAcount, decimal interest, ApplicationUser user, ApplicationDbContext context)
    {
        user.SoDuTaiKhoan += (double)(savingAcount.SoDuSoTietKiem + interest);

        savingAcount.NgayDongSo = DateTime.Now;
        savingAcount.SoDuSoTietKiem = 0;
        savingAcount.TrangThai = false;

        // Lưu thay đổi
        context.Users.Update(user); //cập nhật số dư tài khoản 
        context.SoTietKiems.Update(savingAcount);  // cập nhật số dư STK 
        await context.SaveChangesAsync();
    }
    //Quay vòng gốc là rút lãi và tạo kỳ hạn mới với số tiền gốc ban đầu 
    private static async Task QuayVongGoc(SavingAccount soTietKiem, decimal tienLai, ApplicationUser user, ApplicationDbContext context)
    {

        // Cộng tiền lãi vào tài khoản (chỉ rút lãi)
        user.SoDuTaiKhoan += (double)tienLai;

        // Tạo kỳ hạn mới với số tiền gốc ban đầu
        soTietKiem.NgayMoSo = DateTime.Now;
        var kyHan = soTietKiem.LoaiSoTietKiem.KyHan;
        soTietKiem.NgayDaoHan = soTietKiem.NgayMoSo.AddMonths(kyHan);
        soTietKiem.SoDuSoTietKiem = soTietKiem.SoTienGui; // set lại số dư sổ tiết kiệm là số tiền gửi ban đầu 

        // Lưu thay đổi
        context.Users.Update(user);
        context.SoTietKiems.Update(soTietKiem);
        await context.SaveChangesAsync();
    }

    private static async Task QuayVongGocVaLai(SavingAccount soTietKiem, decimal tienLai, ApplicationDbContext context)
    {
        //Tạo kỳ hạn mới
        soTietKiem.NgayMoSo = DateTime.Now;
        soTietKiem.NgayDaoHan = soTietKiem.NgayMoSo.AddMonths(soTietKiem.LoaiSoTietKiem.KyHan);
        //Số dư = gốc + lãi 
        soTietKiem.SoDuSoTietKiem += tienLai;
        soTietKiem.SoTienGui = soTietKiem.SoDuSoTietKiem;

        // Lưu lại vào database
        context.SoTietKiems.Update(soTietKiem);
        await context.SaveChangesAsync();
    }
}