using System.Diagnostics;

namespace QuanLySoTietKiem.Helpers;
public static class LaiSuatHelper
{
    public const decimal LAI_SUAT_KHONG_KY_HAN = 0.5m;
    //Tính lãi suất gửi tiền

    public static decimal TinhTienLai(decimal depositAmount, decimal interestRate, int soNgayGui)
    {
        var result = depositAmount * (interestRate / 100) * (soNgayGui / 365m);
        return result;
    }

    //Tính lãi suất rút tiền
    public static decimal TinhLaiSuatRutTien(DateTime ngayMoSo, DateTime ngayDaoHan, DateTime ngayRut, decimal laiSuatKyHan)
    {
        if (ngayRut < ngayMoSo)
        {
            throw new Exception("Ngày rút tiền không được nhỏ hơn ngày mở sổ tiết kiệm");
        }
        // Rút đúng ngày đáo hạn
        else if (ngayRut.Date == ngayDaoHan.Date)
        {
            Debug.WriteLine("Case: Exact maturity date withdrawal");
            return laiSuatKyHan;
        }
        else
        {
            Debug.WriteLine("Case: Late withdrawal");
            return LAI_SUAT_KHONG_KY_HAN;
        }

    }

}